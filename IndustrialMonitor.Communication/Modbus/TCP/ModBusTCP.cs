using NModbus;
using NModbus.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Communication.Modbus.TCP
{
    public class ModBusTCP
    {
        private IModbusMaster? _master;
        

        public async Task<TcpClient> ModbusTcpConnectAsync(string ipAddress, int port)
        {
            TcpClient _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(ipAddress, port);

            
            return _tcpClient;
        }

        public async Task<ushort[]> ReadHoldingRegistersAsync(byte slaveId,ushort startAddress,ushort numberOfPoints, TcpClient tcpClient)
        {
            ModbusFactory factory = new ModbusFactory();
            IModbusMaster _master = factory.CreateMaster(tcpClient);

            if (_master == null)
            {
                throw new InvalidOperationException("Modbus Master 尚未初始化，请先连接设备。");
            }

            return await Task.Run(() => _master.ReadHoldingRegisters(slaveId, startAddress, numberOfPoints)); 
        }

    }


}
