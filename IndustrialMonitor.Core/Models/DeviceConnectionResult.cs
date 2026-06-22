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
        public bool IsConnected { get; set; }
        public string Status { get; set; }
        public string? ErrorMessage { get; set; }

    }
}
