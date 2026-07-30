using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Core.Models.DeviceConfigModels
{
    public class SiemensS7DeviceConfig : DeviceConfig
    {
        public required string IpAddress { get; set; } 

        public short Rack { get; set; }

        public short Slot { get; set; } 

        public int DbNumber { get; set; } 
    }
}
