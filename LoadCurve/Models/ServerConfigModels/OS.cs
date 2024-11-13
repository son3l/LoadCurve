using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadCurve.Models.ServerConfigModels
{
    public class OS
    {
        public string Distributor { get; set; }
        public string Release {  get; set; }
        public string CodeName { get; set; }
    }
}
