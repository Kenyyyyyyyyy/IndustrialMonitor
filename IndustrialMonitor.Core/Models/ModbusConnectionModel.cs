using NModbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Core.Models
{
    public class ModbusConnectionModel
    {
        public string IpAddress { get; set; }
        public IModbusMaster modbusMaster { get; set; }
        public TcpClient tcpClient { get; set; }
        public bool IsConnected { get; set; }

        public DeviceConfigModel DeviceConfig { get; set; }
    }
}
