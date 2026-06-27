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
        public Task<DeviceConnectionResult> ConnectAsync(DeviceConfigModel deviceConfig);

        public DeviceConnectionResult DisconnectAsync(DeviceConfigModel deviceConfig);

        Task<ushort[]> ReadHoldingRegistersAsync(string ipAddress,byte slaveId,ushort startAddress,ushort numberOfPoints);

        bool IsConnected(string ipAddress);

        Task<ObservableCollection<DeviceConnectionResult>> ScanipList(ObservableCollection<DeviceConfigModel> deviceConfigCollections);

    }
}
