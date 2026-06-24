using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Core.Models;
using NModbus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Communication.Services
{
    public class DeviceCommunicationService : IDeviceCommunicationService
    {
        private readonly Dictionary<string, ModbusConnectionModel> _connections = [];


        public async Task<bool> ConnectAsync(string ipAddress, int port)
        {
            if (_connections.ContainsKey(ipAddress)) return true;

            TcpClient _tcpClient = new();
            await _tcpClient.ConnectAsync(ipAddress, port);
            ModbusFactory factory = new();

            _connections[ipAddress] = new ModbusConnectionModel
            {
                IpAddress = ipAddress,
                tcpClient = _tcpClient,
                modbusMaster = factory.CreateMaster(_tcpClient),
                IsConnected = true
            };

            return true;
        }

        public Task DisconnectAsync(string ipAddress)
        {
            if(_connections.TryGetValue(ipAddress,out var connection))
            {
                connection.modbusMaster.Dispose();
                connection.tcpClient.Close();
                connection.tcpClient.Dispose();
                _connections.Remove(ipAddress);
            }
            return Task.CompletedTask;
        }


        public bool IsConnected(string ipAddress)
        {
            return _connections.ContainsKey(ipAddress);
        }

        public async Task<ushort[]> ReadHoldingRegistersAsync(string ipAddress, byte slaveId, ushort startAddress, ushort numberOfPoints)
        {
            if (!_connections.TryGetValue(ipAddress, out var connection)) return [];

            try
            {
                ushort[] registers = await Task.Run(() => connection.modbusMaster.ReadHoldingRegisters(slaveId, startAddress, numberOfPoints));
                return registers;
            }
            catch (Exception)
            {
                return Array.Empty<ushort>();
            }
        }

        public async Task<ObservableCollection<DeviceConnectionResult>> ScanipList(Dictionary<string, int> IpPortPairs)
        {
            
            var tasks = IpPortPairs.Select(async pair =>
            {
                try
                {
                    await ConnectAsync(pair.Key, pair.Value);

                    return new DeviceConnectionResult
                    {
                        IpAddress = pair.Key,
                        Port = pair.Value,
                        IsConnected = true,
                        Status = "Connected",
                        ErrorMessage = null

                    };
                }
                catch (Exception ex)
                {
                    return new DeviceConnectionResult
                    {
                        IpAddress = pair.Key,
                        Port = pair.Value,
                        IsConnected = false,
                        Status = "Error",
                        ErrorMessage = ex.Message

                    };
                }
            });

            var results = await Task.WhenAll(tasks);

            return new ObservableCollection<DeviceConnectionResult>(results);

        }
    }
}
