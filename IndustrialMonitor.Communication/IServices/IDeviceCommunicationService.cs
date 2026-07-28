using IndustrialMonitor.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Communication.IServices
{
    public interface IDeviceCommunicationService
    {
        //IReadOnlyDictionary<string, ModbusConnectionModel> Connections { get; }

        public Dictionary<string, ModbusConnectionModel> Connections { get; }
        public Task<DeviceConnectionResult> ConnectAsync(DeviceConfigModel deviceConfig);

        public DeviceConnectionResult DisconnectAsync(DeviceConfigModel deviceConfig);

        Task<ReadRegistersResult> ReadHoldingRegistersAsync(string ipAddress);

        bool IsConnected(string ipAddress);

        ObservableCollection<string> ScanipList();

        public Task<DeviceDataModel> CreateDataModel(string ipAddress);

        Task WriteRegisterAsync(string ipAddress, ushort startAddress, ushort[] values);

        public int GetYield();

        public ushort GetYieldRate();
    }
}
