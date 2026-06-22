using IndustrialMonitor.Core.DeviceDetailModels;
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
        Task<bool> ConnectAsync(string ipAddress, int port);
        Task DisconnectAsync(string ipAddress);

        Task<ushort[]> ReadHoldingRegistersAsync(string ipAddress,byte slaveId,ushort startAddress,ushort numberOfPoints);

        bool IsConnected(string ipAddress);

        Task<ObservableCollection<DeviceConnectionResult>> ScanipList(Dictionary<string, int> IpPortPairs);

    }
}
