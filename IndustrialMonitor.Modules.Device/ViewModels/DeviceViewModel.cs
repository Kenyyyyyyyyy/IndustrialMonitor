using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Communication.Services;
using IndustrialMonitor.Core.Models;
using IndustrialMonitor.DataAcquisition.IServices;
using IndustrialMonitor.Modules.Device.Tools;
using Prism.Commands;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IndustrialMonitor.Modules.Device.ViewModels
{
    public class DeviceViewModel : BindableBase ,INavigationAware
    {

        private readonly DeviceStorageService _deviceStorageService = new();
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDeviceCommunicationService _deviceComunicationService;
        private readonly IAcquisitionService _acquisitionService;

        public DelegateCommand LoadDeviceCmd { get; }
        public DelegateCommand OpenAddCmd { get; }

        public DelegateCommand RefreshCmd { get; }

        public DelegateCommand<DeviceItemViewModel> UpdataCommand { get; }
        public DelegateCommand<DeviceItemViewModel> DeleteCommand { get; }

        public ObservableCollection<DeviceItemViewModel> Devices { get; } = [];
        public List<DeviceConfigModel> DeviceConfigModels = [];


        public DeviceViewModel(IDialogService dialogService,
                               IEventAggregator eventAggregator,
                               IDeviceCommunicationService deviceCommunicationService,
                               IAcquisitionService acquisitionService)
        {
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;
            _deviceComunicationService = deviceCommunicationService;
            _acquisitionService = acquisitionService;

            LoadDeviceCmd = new(async () => await LoadDeviceJson());

            OpenAddCmd = new(() =>
            {
                DialogParameters keyValuePairs = new() 
                { 
                    { "DeviceConfigModels", DeviceConfigModels },
                    { "Mode", "Add" } 
                };

                _dialogService.ShowDialog("DeviceAddWindow", keyValuePairs,async result =>
                {
                    if (result.Result == ButtonResult.OK) 
                    {
                        await LoadDeviceJson();
                    }
                });
            });

            UpdataCommand = new((deviceitem) => 
            {
                if (deviceitem.ConfigModel.IpAddress != null && _deviceComunicationService.IsConnected(deviceitem.ConfigModel.IpAddress))
                {
                    MessageBox.Show("设备正在连接！请断开连接后重试");
                    return;
                }

                DialogParameters keyValuePairs = new()
                {
                    { "DeviceConfigModels", DeviceConfigModels },
                    { "DeviceConfigModel", deviceitem.ConfigModel },
                    { "Mode", "Update" }
                };

                _dialogService.ShowDialog("DeviceAddWindow", keyValuePairs, async result =>
                {
                    if (result.Result == ButtonResult.OK)
                    {
                        await LoadDeviceJson();
                    }
                });

            });

            DeleteCommand = new(async (deviceitem) => await DeleteDevice(deviceitem));

        }


        public async Task LoadDeviceJson()
        {
            Devices.Clear();
            DeviceConfigModels = await _deviceStorageService.LoadDeviceJson();

            foreach (var configmodel in DeviceConfigModels)
            {
                Devices.Add(new DeviceItemViewModel(configmodel, _deviceComunicationService, _acquisitionService));
            }
        }

        

        public async Task DeleteDevice(DeviceItemViewModel deviceitem)
        {
            if (deviceitem.ConfigModel.IpAddress != null && _deviceComunicationService.IsConnected(deviceitem.ConfigModel.IpAddress))
            {
                MessageBox.Show("设备正在连接！请断开连接后重试");
                return;
            }
            Devices.Remove(deviceitem);
            DeviceConfigModels.Remove(deviceitem.ConfigModel);
            await _deviceStorageService.SaveDeviceAsJsonAsync(DeviceConfigModels);

        }


        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        #endregion

        }
}
