using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Core.IRepository;
using IndustrialMonitor.Core.Models;
using IndustrialMonitor.Core.Models.DeviceConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Communication.Services
{
    public class DeviceStorageService : IDeviceStorageService
    {
        private IDeviceStorageRepository _deviceStorageRepository;

        public DeviceStorageService(IDeviceStorageRepository deviceStorageRepository)
        {
            _deviceStorageRepository = deviceStorageRepository;
        }

        public async Task<List<DeviceConfig>> GetDeviceListAsync()
        {
            return await _deviceStorageRepository.LoadDeviceJson();
        }

        public async Task SaveDeviceListAsync(List<DeviceConfig> deviceConfigs)
        {
            await _deviceStorageRepository.SaveDeviceConfigListAsync(deviceConfigs);
        }

        public async Task SaveDeviceConfig(DeviceConfig deviceConfig)
        {
            await _deviceStorageRepository.SaveDeviceModel(deviceConfig);
        }


        public async Task<Guid> GetDeviceIdAsync(string ipAddress)
        {
            return await _deviceStorageRepository.GetDeviceIdAsync(ipAddress);
        }


    }
}
