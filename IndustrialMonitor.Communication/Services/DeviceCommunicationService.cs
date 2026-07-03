using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Core.Models;
using NModbus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Communication.Services
{
    public class DeviceCommunicationService : IDeviceCommunicationService
    {
        private readonly Dictionary<string, ModbusConnectionModel> _connections = [];


        public async Task<DeviceConnectionResult> ConnectAsync(DeviceConfigModel deviceConfig)
        {
            if (_connections.ContainsKey(deviceConfig.IpAddress))
            {
                return new DeviceConnectionResult
                {
                    IpAddress = deviceConfig.IpAddress,
                    Port = deviceConfig.Port,
                    SlaveId = deviceConfig.SlaveId,
                    StartAddress = deviceConfig.StartAddress,
                    NumberOfPoints = deviceConfig.NumberOfPoints,

                    IsConnected = true,
                    Status = "已连接",
                    ErrorMessage = null
                };
            }

            TcpClient _tcpClient = new();

            try
            {
                await _tcpClient.ConnectAsync(deviceConfig.IpAddress, deviceConfig.Port);
            }
            catch (Exception ex)
            {
                return new DeviceConnectionResult
                {
                    IpAddress = deviceConfig.IpAddress,
                    Port = deviceConfig.Port,
                    SlaveId = deviceConfig.SlaveId,
                    StartAddress = deviceConfig.StartAddress,
                    NumberOfPoints = deviceConfig.NumberOfPoints,

                    IsConnected = false,
                    Status = "连接失败",
                    ErrorMessage = ex.Message
                };
            }

            
            ModbusFactory factory = new();

            _connections[deviceConfig.IpAddress] = new ModbusConnectionModel
            {
                IpAddress = deviceConfig.IpAddress,
                tcpClient = _tcpClient,
                modbusMaster = factory.CreateMaster(_tcpClient),
                IsConnected = true,
                DeviceConfig = deviceConfig
            };

            return new DeviceConnectionResult
            {
                IpAddress = deviceConfig.IpAddress,
                Port = deviceConfig.Port,
                SlaveId = deviceConfig.SlaveId,
                StartAddress = deviceConfig.StartAddress,
                NumberOfPoints = deviceConfig.NumberOfPoints,

                IsConnected = true,
                Status = "已连接",
                ErrorMessage = null
            }; 
        }

        public DeviceConnectionResult DisconnectAsync(DeviceConfigModel deviceConfig)
        {
            if(_connections.TryGetValue(deviceConfig.IpAddress, out var connection))
            {
                connection.modbusMaster.Dispose();
                connection.tcpClient.Close();
                connection.tcpClient.Dispose();
                _connections.Remove(deviceConfig.IpAddress);
            }
            return new DeviceConnectionResult
            {
                IpAddress = deviceConfig.IpAddress,
                Port = deviceConfig.Port,
                SlaveId = deviceConfig.SlaveId,
                StartAddress = deviceConfig.StartAddress,
                NumberOfPoints = deviceConfig.NumberOfPoints,

                IsConnected = false,
                Status = "已断开",
                ErrorMessage = null
            };
        }


        public bool IsConnected(string ipAddress)
        {
            return _connections.ContainsKey(ipAddress);
        }

        public async Task<ushort[]> ReadHoldingRegistersAsync(string ipAddress)
        {
            if (!_connections.TryGetValue(ipAddress, out var connection)) return [];


            

            try
            {
                ushort[] registers = await Task.Run(
                    () => connection.modbusMaster.ReadHoldingRegisters(_connections[ipAddress].DeviceConfig.SlaveId, _connections[ipAddress].DeviceConfig.StartAddress, _connections[ipAddress].DeviceConfig.NumberOfPoints));
                return registers;
            }
            catch (Exception)
            {
                return [];
            }
        }

        public ObservableCollection<string> ScanipList()
        {

            ////var tasks = deviceConfigCollections.Select(async deviceConfig =>
            ////{
            ////    return await ConnectAsync(deviceConfig);
            ////});

            //var tasks = _connections.Select(connection =>
            //{
            //    return connection.Key;
            //});



            //var results = await Task.WhenAll(tasks);

            //return new ObservableCollection<DeviceConnectionResult>(results);
            ObservableCollection<string> conlist = [];

            foreach (string ipAddress in _connections.Keys)
            {
                conlist.Add(ipAddress);
            }

            return conlist;

        }
    }
}
