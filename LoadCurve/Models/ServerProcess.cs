using System;

namespace LoadCurve.Models;

public class ServerProcess
{
    public string UserName { get; set; }
    public string PID { get; set; }
    public double CPUUsage { get; set; }
    public double MemoryUsage { get; set; }
    public string ProcessName { get; set; }
    public string Status { get; set; }
    public string CPUUsageTime { get; set; }
}