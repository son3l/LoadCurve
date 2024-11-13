using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadCurve.Models.ServerConfigModels
{
    public class ServerConfigInfo
    {
        public Processor Processor { get; set; }
        public double TotalMemory { get; set; }
        public List<Disk> Disks { get; set; }
        public OS OS { get; set; }
    }
}
