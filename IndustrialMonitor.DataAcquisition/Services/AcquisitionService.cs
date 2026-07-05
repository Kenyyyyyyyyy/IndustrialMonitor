using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Core.IRepository;
using IndustrialMonitor.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.DataAcquisition.Services
{
    public class AcquisitionService : IAcquisitionService
    {
        private readonly IBaseRepository<DeviceDataModel> _baseRepository;
        private readonly IDeviceCommunicationService _deviceCommunicationService;
        public AcquisitionService(IBaseRepository<DeviceDataModel> baseRepository, IDeviceCommunicationService deviceCommunicationService) 
        {
            _baseRepository = baseRepository;
            _deviceCommunicationService = deviceCommunicationService;
        }

        public async Task WriteInResister(string ipAddress)
        {
            DeviceDataModel deviceDataModels = await _deviceCommunicationService.CreateDataModel(ipAddress);

            try
            {
                await _baseRepository.RecordData(deviceDataModels);
                await Task.Delay(5000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to device {ipAddress}: {ex.Message}");
            }
        }

    }
}
