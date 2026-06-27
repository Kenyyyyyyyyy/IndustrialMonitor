using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Core.Models
{
    public class DeviceConnectionResult
    {
        public string IpAddress { get; set; } 
        public int Port { get; set; }

        public byte SlaveId { get; set; }

        public ushort StartAddress { get; set; }

        public ushort NumberOfPoints { get; set; }

        public bool IsConnected { get; set; }
        public string Status { get; set; } = "未连接";
        public string? ErrorMessage { get; set; }

    }
}
