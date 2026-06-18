using NModbus;
using System.Net.Sockets;

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
            ushort[] registers = await Task.Run(() => _master.ReadHoldingRegisters(slaveId, startAddress, numberOfPoints));
            return registers;
        }

    }


}
