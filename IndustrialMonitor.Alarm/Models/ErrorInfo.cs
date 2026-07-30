using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Alarm.Models
{
    public class ErrorInfo
    {
        public Guid? deviceid;

        public required Exception exception;
        public string? IpAddress { get; set; }

        public byte? SlaveId { get; set; }

        public ushort? Port { get; set; }
    }
}
