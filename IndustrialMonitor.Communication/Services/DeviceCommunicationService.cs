using IndustrialMonitor.Alarm.IRepository;
using IndustrialMonitor.Alarm.Models;
using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Core.IRepository;
using IndustrialMonitor.Core.Models;
using IndustrialMonitor.EventSupport.IServices;
using IndustrialMonitor.EventSupport.Services;
using NModbus;
using NModbus.Device;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace IndustrialMonitor.Communication.Services
{
    public class DeviceCommunicationService : IDeviceCommunicationService
    {
        private readonly IEventPublishService _eventPublishService;

        private readonly Dictionary<string, ModbusConnectionModel> _connections = [];

        public Dictionary<string, ModbusConnectionModel> Connections
        {
            get { return _connections; }
        }

        

        public DeviceCommunicationService(IEventPublishService eventPublishService)
        {
            _eventPublishService = eventPublishService;
        }

        

        public async Task<DeviceConnectionResult> ConnectAsync(DeviceConfigModel deviceConfig)
        {
            if (_connections.ContainsKey(deviceConfig.IpAddress))
            {
                return new DeviceConnectionResult
                {
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

                _eventPublishService.PublishCommunicationErrorInfo(
                    deviceConfig.Id,
                    deviceConfig.IpAddress,
                    deviceConfig.SlaveId,
                    deviceConfig.Port,
                    ex);

                return new DeviceConnectionResult
                {
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
                DeviceConfig = deviceConfig,
                
            };

            _connections[deviceConfig.IpAddress].modbusMaster.Transport.ReadTimeout = 1000;
            _connections[deviceConfig.IpAddress].modbusMaster.Transport.WriteTimeout = 1000;
            _connections[deviceConfig.IpAddress].modbusMaster.Transport.Retries = 0;


            try
            {
                await Task.Run(() =>
                _connections[deviceConfig.IpAddress].modbusMaster.ReadHoldingRegisters(deviceConfig.SlaveId,deviceConfig.StartAddress,1));
                
            }
            

            catch (Exception ex)
            {
                _eventPublishService.PublishCommunicationErrorInfo(
                    deviceConfig.Id,
                    deviceConfig.IpAddress,
                    deviceConfig.SlaveId,
                    deviceConfig.Port,
                    ex);

                DisconnectAsync(deviceConfig);


                return new DeviceConnectionResult
                {
                    IsConnected = false,
                    Status = "Modbus验证失败",
                    ErrorMessage = ex.Message
                };
            }

            return new DeviceConnectionResult
            {
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
                IsConnected = false,
                Status = "已断开",
                ErrorMessage = null
            };
        }

        public bool IsConnected(string ipAddress)
        {
            return _connections.ContainsKey(ipAddress);
        }

        public async Task<ReadRegistersResult> ReadHoldingRegistersAsync(string ipAddress)
        {
            if (!_connections.TryGetValue(ipAddress, out var connection)) 

                

                return new ReadRegistersResult
                {
                    Success = false,
                    Data = [],
                    ErrorMessage = "设备未连接",
                    IsConnected = false
                };

            DeviceConfigModel deviceConfig = connection.DeviceConfig;

            try
            {
                ushort[] registers = await Task.Run(
                    () => connection.modbusMaster
                    .ReadHoldingRegisters(deviceConfig.SlaveId, deviceConfig.StartAddress, deviceConfig.NumberOfPoints));

                return new ReadRegistersResult
                {
                    Success = true,
                    Data = registers,
                    ErrorMessage = null,
                    IsConnected = true
                };
            }

            catch (Exception ex)
            {
                _eventPublishService.PublishCommunicationErrorInfo(
                   deviceConfig.Id,
                   deviceConfig.IpAddress,
                   deviceConfig.SlaveId,
                   deviceConfig.Port,
                   ex);

                DisconnectAsync(_connections[ipAddress].DeviceConfig);

                return new ReadRegistersResult
                {
                    Success = false,
                    Data = [],
                    ErrorMessage = "Exception错误" + ex.Message,
                    IsConnected = false
                };
            }
        }

        public ObservableCollection<string> ScanipList()
        {
            ObservableCollection<string> conlist = [];

            foreach (string ipAddress in _connections.Keys)
            {
                conlist.Add(ipAddress);
            }

            return conlist;
        }

        public async Task<DeviceDataModel> CreateDataModel(string ipAddress)
        {
            var result = await ReadHoldingRegistersAsync(ipAddress);

            if (result.Success == false)
            {
                throw new InvalidOperationException(result.ErrorMessage);
            }

            DateTime now = DateTime.Now;
            DateTime time = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

            DeviceDataModel deviceDataModel = new()
            {
                DeviceId = _connections[ipAddress].DeviceConfig.Id,
                DateTime = time
            };

            deviceDataModel.SetValues(result.Data);

            return deviceDataModel;
        }

        public Task WriteRegisterAsync(string ipAddress,ushort startAddress,ushort[] values)
        {
            if (!_connections.TryGetValue(ipAddress, out var connection))
            {
                throw new InvalidOperationException($"设备未连接：{ipAddress}");
            }

            connection.modbusMaster.WriteMultipleRegisters(
                connection.DeviceConfig.SlaveId,
                startAddress,
                values);

            return Task.CompletedTask;
        }

        public int GetYield()
        {
            Allyield = 0;
            foreach (var connectionModel in Connections.Values)
            {
                DeviceConfigModel deviceConfigModel = connectionModel.DeviceConfig;
                try 
                {
                    ushort[] registers = connectionModel.modbusMaster.ReadHoldingRegisters
                    (deviceConfigModel.SlaveId, 15, 1);
                    ushort value = registers[0];
                    Allyield += value;
                }
                catch (Exception ex)
                {
                    _eventPublishService.PublishCommunicationErrorInfo(
                   deviceConfigModel.Id,
                   deviceConfigModel.IpAddress,
                   deviceConfigModel.SlaveId,
                   deviceConfigModel.Port,
                   ex);

                    continue;
                }
            }

            return Allyield;
        }

        public ushort GetYieldRate()
        {
            int goods = 0;

            foreach (var connectionModel in Connections.Values)
            {
                DeviceConfigModel deviceConfigModel = connectionModel.DeviceConfig;
                try
                {
                    ushort[] registers = connectionModel.modbusMaster.ReadHoldingRegisters
                    (deviceConfigModel.SlaveId, 16, 1);
                    ushort value = registers[0];
                    goods += value;
                }
                catch (Exception ex)
                {
                    _eventPublishService.PublishCommunicationErrorInfo(
                  deviceConfigModel.Id,
                  deviceConfigModel.IpAddress,
                  deviceConfigModel.SlaveId,
                  deviceConfigModel.Port,
                  ex);

                    continue;
                }
            }

            if (Allyield == 0) return 0;
            

            return (ushort)(((double)goods / Allyield) * 100) ;
        }


        private int _allyield;

        public int Allyield
        {
            get { return _allyield; }
            set { _allyield = value; }
        }

    }
}
