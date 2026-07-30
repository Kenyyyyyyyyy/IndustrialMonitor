using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Core.Models.DeviceConfigModels
{
    public class ModbusRtuDeviceConfig : DeviceConfig
    {
        public required string ComPort { get; set; } 

        public int BaudRate { get; set; } 

        public byte DataBits { get; set; } 

        public byte SlaveId { get; set; } 

        public required string Parity { get; set; } 
    }
}
