using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Communication.Services;
using IndustrialMonitor.Core.Models;
using IndustrialMonitor.DataAcquisition.IServices;
using IndustrialMonitor.DataAcquisition.Services;
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
        private IAcquisitionService _acquisitionservice { get; }
        public DeviceItemViewModel(DeviceConfigModel configModel, 
                                   IDeviceCommunicationService deviceCommunicationService, 
                                   IAcquisitionService acquisitionService)
        {
            ConfigModel = configModel;
            DeviceCommunicationService = deviceCommunicationService;
            _acquisitionservice = acquisitionService;
            _ = ConnectAsync();

            ConnectCommand = new(async () => await ConnectAsync());
            DisconnectCommand = new(() => DisconnectAsync());

        }

        public async Task ConnectAsync()
        {
            ConnectionResult = await DeviceCommunicationService.ConnectAsync(ConfigModel);

            if (ConnectionResult.IsConnected)
            {
                await _acquisitionservice.StartCollectAsync(ConfigModel.IpAddress);
            }
        }

        public void DisconnectAsync()
        {
            if (ConnectionResult.IsConnected)
            {
                _acquisitionservice.StopCollectAsync(ConfigModel.IpAddress);
            }
            ConnectionResult = DeviceCommunicationService.DisconnectAsync(ConfigModel);
        }
    }
}
