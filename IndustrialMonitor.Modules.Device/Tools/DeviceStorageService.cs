using IndustrialMonitor.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IndustrialMonitor.Modules.Device.Tools
{
    public class DeviceStorageService: IDeviceStorageService
    {

        private readonly string _filePath;

        public DeviceStorageService() 
        {
            string directory = @"C:\Users\56409\source\repos\Kenyyyyyyyyy\IndustrialMonitor\DeviceJson";
            //string directory = @"C:\Users\ADMIN\source\repos\IndustrialMonitor\DeviceJson";
            Directory.CreateDirectory(directory);
            _filePath = Path.Combine(directory, "device.json");
        }

        public async Task SaveDeviceAsJsonAsync(List<DeviceConfigModel> deviceObservable) 
        {
            var json = JsonSerializer.Serialize(deviceObservable);
            await File.WriteAllTextAsync(_filePath, json);
        }


        public async Task<List<DeviceConfigModel>> LoadDeviceJson()
        {
            if (!File.Exists(_filePath)) return [];

            var json = await File.ReadAllTextAsync(_filePath);

            if (json != null)
            {
                return JsonSerializer.Deserialize<List<DeviceConfigModel>>(json) ?? [];
                
            }
            return [];

        }

        public async Task<string> GetDeviceIdAsync(string ipAddress)
        {
            var json = await File.ReadAllTextAsync(_filePath);

            List <DeviceConfigModel> deviceConfigModels = JsonSerializer.Deserialize<List<DeviceConfigModel>>(json) ?? [];

            if (deviceConfigModels == null || deviceConfigModels.Count == 0)
            {
                return string.Empty;
            }

            var deviceConfig = deviceConfigModels.Find(x => x.IpAddress == ipAddress);

            if (deviceConfig == null)
            {
                return string.Empty;
            }

            return deviceConfig.Id.ToString();
        }
    }
}
