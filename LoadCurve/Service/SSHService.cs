using LoadCurve.Models;
using LoadCurve.Models.ServerConfigModels;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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
        catch (Exception e)
        {
            _Client.Dispose();
            throw new Exception($"Failed to connect to the SSH server: {e.Message}");
        }
    }
    public ServerConfigInfo GetServerConfig()
    {
        ServerConfigInfo config = new();
        using SshCommand CPUcommand = _Client.RunCommand("lscpu");
        config.Processor = ParseProcessorInfo(CPUcommand.Result);
        using SshCommand Memorycommand = _Client.RunCommand("free -h");
        config.TotalMemory = ParseMemoryInfo(Memorycommand.Result);
        using SshCommand Diskcommand = _Client.RunCommand("df -h");
        config.Disks = [.. ParseDisks(Diskcommand.Result)];
        using SshCommand OScommand = _Client.RunCommand("lsb_release -a");
        config.OS = ParseOS(OScommand.Result);
        return config;
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
        ServerLoadInfo.TotalMemoryUsage = double.Round(ServerLoadInfo.TotalMemoryUsage / ServerLoadInfo.MemoryHeap * 100, 2);
        ServerLoadInfo.Processes.AddRange(result);
        return ServerLoadInfo;
    }

    public void CloseConnection()
    {
        _Client.Disconnect();
        _Client.Dispose();
    }
    private OS ParseOS(string CommandResult) 
    {
        OS oS = new();
        string[] fields = CommandResult.Split("\n");
        oS.Distributor = fields[0].Split(":")[1].Trim();
        oS.Release = fields[2].Split(":")[1].Trim();
        oS.CodeName = fields[3].Split(":")[1].Trim();
        return oS;
    }
    private List<Disk> ParseDisks(string CommandResult)
    {
        List<Disk> disks = new();
        string[] fields = CommandResult.Split("\n");
        foreach (var field in fields.Where(field=>field.Contains("/dev/vd")))
        {
            string[] properties = field.Split(" ");
            disks.Add(new Disk() 
            {
                Name = properties[0].Trim(),
                Size = properties.First(value=> !string.IsNullOrWhiteSpace(value)&&!value.Contains("/dev/"))
            });
        }
        return disks;
    }
    private double ParseMemoryInfo(string CommandResult)
    {
        string[] fields = CommandResult.Split("\n");
        return double.Parse(fields[1].Split(":")[1].Split("G")[0].Trim().Replace(".",","));   
    }
    private Processor ParseProcessorInfo(string CommandResult)
    {
        Processor processor = new Processor();
        string[] fields = CommandResult.Split("\n");
        processor.Architecture = fields.First(value=>value.Contains("Architecture")).Split(":")[1].Trim();
        processor.CPU = int.Parse(fields.First(value => value.Contains("CPU(s)")).Split(":")[1]);
        processor.ThreadsPerCore = int.Parse(fields.First(value => value.Contains("Thread(s) per core")).Split(":")[1]);
        processor.CorePerSocket = int.Parse(fields.First(value => value.Contains("Core(s) per socket")).Split(":")[1]);
        processor.Sockets = int.Parse(fields.First(value => value.Contains("Socket(s)")).Split(":")[1]);
        processor.ModelName = fields.First(value => value.Contains("Model name")).Split(":")[1].Trim();
        return processor;
    }

    private List<ServerProcess> ParseProcesses(string CommandResult)
    {
        List<ServerProcess> Processes = new();
        List<string> splitCommandResult = [.. CommandResult.Split("\n")];
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
                    CPUUsage = double.Parse(processData[2].Replace(".", ",")) / ServerLoadInfo.CoreNumbers,
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