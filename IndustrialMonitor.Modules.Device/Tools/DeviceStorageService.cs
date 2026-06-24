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
    public class DeviceStorageService
    {

        private readonly string _filePath;

        public DeviceStorageService() 
        {
            string directory = @"C:\Users\56409\source\repos\Kenyyyyyyyyy\IndustrialMonitor\DeviceJson";
            Directory.CreateDirectory(directory);
            _filePath = Path.Combine(directory, "device.json");
        }

        public async Task SaveDeviceAsJsonAsync(ObservableCollection<DeviceConfigModel> deviceObservable) 
        {
            var json = JsonSerializer.Serialize(deviceObservable);
            await File.WriteAllTextAsync("_filePath", json);
        }


        public async Task<ObservableCollection<DeviceConfigModel>> LoadDeviceJson()
        {
            if (!File.Exists("_filePath")) return [];

            var json = await File.ReadAllTextAsync("_filePath");

            if (json != null)
            {
                return JsonSerializer.Deserialize<ObservableCollection<DeviceConfigModel>>(json) ?? [];
                
            }
            return [];

        }
    }
}
