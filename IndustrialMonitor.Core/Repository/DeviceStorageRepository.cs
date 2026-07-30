using IndustrialMonitor.Core.IRepository;
using IndustrialMonitor.Core.Models;
using IndustrialMonitor.Core.Models.DeviceConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IndustrialMonitor.Core.Repository
{
    public class DeviceStorageRepository : IDeviceStorageRepository
    {
        private readonly string _filePath;

        public DeviceStorageRepository()
        {
            string directory = @"C:\Users\56409\source\repos\Kenyyyyyyyyy\IndustrialMonitor\DeviceJson";
            //string directory = @"C:\Users\ADMIN\source\repos\IndustrialMonitor\DeviceJson";
            Directory.CreateDirectory(directory);
            _filePath = Path.Combine(directory, "device.json");
        }


        public async Task SaveDeviceModel(DeviceConfig deviceConfig)
        {
            var json = await File.ReadAllTextAsync(_filePath);
            List<DeviceConfig> deviceConfigModels = JsonSerializer.Deserialize<List<DeviceConfig>>(json) ?? [];
            deviceConfigModels.Add(deviceConfig);
            json = JsonSerializer.Serialize(deviceConfigModels);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task SaveDeviceConfigListAsync(List<DeviceConfig> deviceConfigs)
        {
            var json = JsonSerializer.Serialize(deviceConfigs);
            await File.WriteAllTextAsync(_filePath, json);
        }


        public async Task<List<DeviceConfig>> LoadDeviceJson()
        {
            if (!File.Exists(_filePath)) return [];

            var json = await File.ReadAllTextAsync(_filePath);

            if (json != null)
            {
                return JsonSerializer.Deserialize<List<DeviceConfig>>(json) ?? [];

            }
            return [];

        }



        public async Task<Guid> GetDeviceIdAsync(string ipAddress)
        {
            var json = await File.ReadAllTextAsync(_filePath);

            List<DeviceConfigModel> deviceConfigModels = JsonSerializer.Deserialize<List<DeviceConfigModel>>(json) ?? [];

            if (deviceConfigModels == null || deviceConfigModels.Count == 0)
            {
                return Guid.Empty;
            }

            var deviceConfig = deviceConfigModels.Find(x => x.IpAddress == ipAddress);

            if (deviceConfig == null)
            {
                return Guid.Empty;
            }

            return deviceConfig.Id;
        }
    }
}
