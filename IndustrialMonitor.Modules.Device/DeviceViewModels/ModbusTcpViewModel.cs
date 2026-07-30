using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Core.Models;
using IndustrialMonitor.Core.Models.DeviceConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Modules.Device.DeviceViewModels
{
    public class ModbusTcpViewModel : BindableBase
    {
        private readonly IDeviceStorageService _deviceStorageService;

        private ModbusTcpDeviceConfig _modbusTcpDeviceConfig = new();

        public ModbusTcpDeviceConfig ModbusTcpDeviceConfig
        {
            get => _modbusTcpDeviceConfig;
            set => SetProperty(ref _modbusTcpDeviceConfig, value);
        }

        public DelegateCommand AddDeviceCommand { get; set; }

        public ModbusTcpViewModel(IDeviceStorageService deviceStorageService)
        {
            _deviceStorageService = deviceStorageService;

            AddDeviceCommand = new(() =>
            {
                _deviceStorageService.SaveDeviceConfig(ModbusTcpDeviceConfig);
            });
        }

        
    }
}
