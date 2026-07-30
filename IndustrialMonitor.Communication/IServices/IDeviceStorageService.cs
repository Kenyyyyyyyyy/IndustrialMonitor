using IndustrialMonitor.Core.Models;
using IndustrialMonitor.Core.Models.DeviceConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Communication.IServices
{
    public interface IDeviceStorageService
    {
        public Task<List<DeviceConfig>> GetDeviceListAsync();

        public Task SaveDeviceConfig(DeviceConfig deviceConfig);

        public Task SaveDeviceListAsync(List<DeviceConfig> deviceConfigs);

        public Task<Guid> GetDeviceIdAsync(string ipAddress);
    }
}
