using System.Collections.Generic;

namespace LoadCurve.Models;

public class ServerLoadInfo
{
    public List<ServerProcess> Processes { get; set; }
    public double TotalCPUUsage { get; set; }
    public double TotalMemoryUsage { get; set; }
}