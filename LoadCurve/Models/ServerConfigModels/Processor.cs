using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadCurve.Models.ServerConfigModels
{
    public class Processor
    {
        public string Architecture { get; set; }
        public int CPU { get; set; }
        public int ThreadsPerCore { get; set; }
        public int CorePerSocket { get; set; }
        public int Sockets { get; set; }
        public string ModelName { get; set; }
    }
}
