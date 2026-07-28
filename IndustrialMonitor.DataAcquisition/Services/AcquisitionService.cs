using IndustrialMonitor.Alarm.Models;
using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Core.IRepository;
using IndustrialMonitor.Core.Models;
using IndustrialMonitor.DataAcquisition.IServices;
using IndustrialMonitor.EventSupport.IServices;
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
        private readonly IModbusSimulationService _modbusSimulationService;
        private readonly IEventPublishService _eventPublishService;

        public readonly Dictionary<string, CancellationTokenSource> _tokens = [];


        public AcquisitionService(
            IBaseRepository<DeviceDataModel> baseRepository,
            IDeviceCommunicationService deviceCommunicationService,
            IModbusSimulationService modbusSimulationService,
            IEventPublishService eventPublishService) 
        {
            _baseRepository             = baseRepository;
            _deviceCommunicationService = deviceCommunicationService;
            _modbusSimulationService    = modbusSimulationService;
            _eventPublishService        = eventPublishService;
        }


        public Task StartCollectAsync(string ipAddress, Guid deviceid)
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
            _ = WriteInResisterAsync(ipAddress, deviceid);

            return Task.CompletedTask;
        }

        public void StopCollectAsync(string ipAddress) 
        {
            if (!_tokens.TryGetValue(ipAddress,out var cts)) return;
            cts.Cancel();
            cts.Dispose();
            _tokens.Remove(ipAddress);
        }

        private async Task WriteInResisterAsync(string ipAddress ,Guid deviceid)
        {
            if (!_tokens.TryGetValue(ipAddress, out var _cts)) return;

            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    DeviceDataModel deviceDataModels = await _deviceCommunicationService.CreateDataModel(ipAddress);
                    await _baseRepository.RecordData(deviceDataModels);
                    _ = _modbusSimulationService.StartSimulation(ipAddress);
                    await Task.Delay(10000, _cts.Token);
                }
                
                catch (Exception ex)
                {
                    _eventPublishService.PublishErrorInfo(deviceid, ex); //找一下deviceid 发布 
                    StopCollectAsync(ipAddress);
                    return;
                }
            }
        }

        
    }
}
