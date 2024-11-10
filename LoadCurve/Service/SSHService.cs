using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using LoadCurve.Models;
using Renci.SshNet;

namespace LoadCurve.Service;

public class SSHService
{
    private SshClient _Client;

    public SSHService(Server server)
    {
        _Client = new SshClient(new ConnectionInfo(server.Address, server.UserName,
            new PasswordAuthenticationMethod(server.UserName, server.Password)));
        TryConnect();
    }
    private void TryConnect()
    {
        try
        {
            _Client.Connect();
        }
        catch(Exception e)
        {
            _Client.Dispose();
            throw new Exception($"Failed to connect to the SSH server: {e.Message}");
        }
    }

    public ServerLoadInfo GetServerLoadInfo()
    {
        using SshCommand command = _Client.RunCommand("ps aux");
        List<ServerProcess> result = ParseProcesses(command.Result);
        ServerLoadInfo info = new();
        foreach (var process in result)
        {
            info.TotalMemoryUsage += process.MemoryUsage;
            info.TotalCPUUsage += process.CPUUsage;
        }
        info.Processes.AddRange(result);
        return info;
    }

    private List<ServerProcess> ParseProcesses(string CommandResult)
    {
        List<ServerProcess> Processes = new();
        List<string> splitCommandResult = [..CommandResult.Split("\n")];
        splitCommandResult.RemoveAt(0);
        foreach (var processInfo in splitCommandResult)
        {
            string[] processData = processInfo.Split(" ").Where(info=>!string.IsNullOrWhiteSpace(info)).ToArray();
            Processes.Add(new ServerProcess()
            {
                UserName = processData[0],
                PID= processData[1],
                CPUUsage = double.Parse(processData[2].Replace(".", ",")),
                MemoryUsage = double.Parse(processData[3].Replace(".", ",")),
                Status = processData[7],
                CPUUsageTime = processData[9],
                ProcessName = processData[10]      
            });
        }
        return Processes;
    }
}