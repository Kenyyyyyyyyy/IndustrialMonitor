using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Core.Models
{
    public class ReadRegistersResult
    {
        public bool Success { get; set; }

        public ushort[] Data { get; set; } = [];

        public string? ErrorMessage { get; set; }

        public bool IsConnected { get; set; }
    }
}
