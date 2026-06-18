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

            return await Task.Run(() => _master.ReadHoldingRegisters(slaveId, startAddress, numberOfPoints)); 

        }

    }


}
