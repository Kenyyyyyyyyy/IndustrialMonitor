using IndustrialMonitor.Core.Models;
using IndustrialMonitor.Core.Models.DeviceConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Core.IRepository
{
    public interface IDeviceStorageRepository
    {
        
        public Task SaveDeviceModel(DeviceConfig deviceConfig);

        public Task SaveDeviceConfigListAsync(List<DeviceConfig> deviceConfigList);

        public Task<List<DeviceConfig>> LoadDeviceJson();

        public Task<Guid> GetDeviceIdAsync(string ipAddress);

        
    }
}
