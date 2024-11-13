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
    private ServerLoadInfo ServerLoadInfo;

    public SSHService(Server server)
    {
        _Client = new SshClient(new ConnectionInfo(server.Address, server.UserName,
            new PasswordAuthenticationMethod(server.UserName, server.Password)));
        TryConnect();
        ServerLoadInfo = new();
        GetPowerStats();
    }
    private void GetPowerStats() 
    {
        using SshCommand NPROCcommand = _Client.RunCommand("nproc");
        ServerLoadInfo.CoreNumbers = int.Parse(NPROCcommand.Result);
        using SshCommand CATCcommand = _Client.RunCommand("cat /proc/meminfo");
        ServerLoadInfo.MemoryHeap = int.Parse(CATCcommand.Result.Split(":")[1].Split("k")[0].Trim());
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
        ServerLoadInfo.Processes = new();
        ServerLoadInfo.TotalCPUUsage = 0;
        ServerLoadInfo.TotalMemoryUsage = 0;
        using SshCommand PScommand = _Client.RunCommand("ps aux --sort=-%cpu");
        List<ServerProcess> result = ParseProcesses(PScommand.Result);
        foreach (var process in result)
        {
            ServerLoadInfo.TotalMemoryUsage += process.ResidentMemoryUsage;
            ServerLoadInfo.TotalCPUUsage += process.CPUUsage;
        }
        ServerLoadInfo.TotalCPUUsage /= ServerLoadInfo.CoreNumbers;
        ServerLoadInfo.TotalMemoryUsage = double.Round((ServerLoadInfo.TotalMemoryUsage/ServerLoadInfo.MemoryHeap)*100,2);
        ServerLoadInfo.Processes.AddRange(result);
        return ServerLoadInfo;
    }

    public void CloseConnection()
    {
        _Client.Disconnect();
        _Client.Dispose();
    }

    private List<ServerProcess> ParseProcesses(string CommandResult)
    {
        List<ServerProcess> Processes = new();
        List<string> splitCommandResult = [..CommandResult.Split("\n")];
        splitCommandResult.RemoveAt(0);
        foreach (var processInfo in splitCommandResult)
        {
            if (processInfo != "")
            {
                string[] processData = processInfo.Split(" ").Where(info => !string.IsNullOrWhiteSpace(info)).ToArray();
                Processes.Add(new ServerProcess()
                {
                    UserName = processData[0],
                    PID = processData[1],
                    CPUUsage = double.Parse(processData[2].Replace(".", ",")),
                    MemoryUsage = double.Parse(processData[3].Replace(".", ",")),
                    ResidentMemoryUsage = double.Parse(processData[5]),
                    Status = processData[7],
                    CPUUsageTime = processData[9],
                    ProcessName = processData[10]
                });
            }
        }
        return Processes;
    }
}