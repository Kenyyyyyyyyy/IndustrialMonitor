using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Communication.Services;
using IndustrialMonitor.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IndustrialMonitor.Modules.Device.ViewModels
{
    public class DeviceItemViewModel: BindableBase
    {
        public DeviceConfigModel ConfigModel { get; }
        

        private DeviceConnectionResult _connectionResult;

        public DeviceConnectionResult ConnectionResult
        {
            get => _connectionResult; 
            set => SetProperty(ref _connectionResult, value);
        }


        public DelegateCommand ConnectCommand { get; }
        public DelegateCommand DisconnectCommand { get; }

        private IDeviceCommunicationService DeviceCommunicationService { get; }
        public DeviceItemViewModel(DeviceConfigModel configModel, IDeviceCommunicationService deviceCommunicationService)
        {
            ConfigModel = configModel;
            DeviceCommunicationService = deviceCommunicationService;
            _ = ConnectAsync();

            ConnectCommand = new(async () => await ConnectAsync());

            DisconnectCommand = new(() => DisconnectAsync());
        }

        public async Task ConnectAsync()
        {
            if(DeviceCommunicationService.IsConnected(ConfigModel.IpAddress))
            {
                return;
            }
            ConnectionResult = await DeviceCommunicationService.ConnectAsync(ConfigModel);
        }

        public void DisconnectAsync()
        {
            ConnectionResult = DeviceCommunicationService.DisconnectAsync(ConfigModel);
        }
    }
}
