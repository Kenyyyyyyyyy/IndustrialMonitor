using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Core.IRepository;
using IndustrialMonitor.Core.Models;
using IndustrialMonitor.DataAcquisition.IServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.DataAcquisition.Services
{
    public class AcquisitionService : IAcquisitionService
    {
        private readonly IBaseRepository<DeviceDataModel> _baseRepository;
        private readonly IDeviceCommunicationService _deviceCommunicationService;

        public readonly Dictionary<string, CancellationTokenSource> _tokens = [];


        public AcquisitionService(IBaseRepository<DeviceDataModel> baseRepository, IDeviceCommunicationService deviceCommunicationService) 
        {
            _baseRepository = baseRepository;
            _deviceCommunicationService = deviceCommunicationService;
        }

        public Task StartCollectAsync(string ipAddress)
        {
            if (_tokens.TryGetValue(ipAddress, out _))
            {
                if (!_tokens[ipAddress].IsCancellationRequested)
                {
                    return Task.CompletedTask;
                }
            }
            CancellationTokenSource _cts = new();
            _tokens[ipAddress] = _cts;
            _ = WriteInResisterAsync(ipAddress);

            return Task.CompletedTask;
        }

        public void StopCollectAsync(string ipAddress) 
        {
            if (!_tokens.TryGetValue(ipAddress,out var cts)) return;
            cts.Cancel();
            cts.Dispose();
            _tokens.Remove(ipAddress);
        }

        private async Task WriteInResisterAsync(string ipAddress)
        {
            if (!_tokens.TryGetValue(ipAddress, out var _cts)) return;

            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    DeviceDataModel deviceDataModels = await _deviceCommunicationService.CreateDataModel(ipAddress);
                    await _baseRepository.RecordData(deviceDataModels);
                    await Task.Delay(5000, _cts.Token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (IOException)
                {
                    return;
                }
                catch (SocketException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error writing to device {ipAddress}: {ex.Message}");
                    StopCollectAsync(ipAddress);
                    return;
                }
            }
        }
    }
}
