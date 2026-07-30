using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Communication.Services;
using IndustrialMonitor.Core.Models;
using IndustrialMonitor.Core.Models.DeviceConfigModels;
using IndustrialMonitor.DataAcquisition.IServices;
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

        
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;

        private readonly IDeviceCommunicationService _deviceComunicationService;
        private readonly IAcquisitionService _acquisitionService;
        private readonly IDeviceStorageService _deviceStorageService;

        private readonly IS7CommunicationService _s7CommunicationService;

        public DelegateCommand LoadDeviceCmd { get; }
        public DelegateCommand OpenAddCmd { get; }

        public DelegateCommand RefreshCmd { get; }

        public DelegateCommand<DeviceItemViewModel> UpdataCommand { get; }
        public DelegateCommand<DeviceItemViewModel> DeleteCommand { get; }

        public DelegateCommand PLCConnectCmd { get; }

        public ObservableCollection<DeviceItemViewModel> Devices { get; } = [];
        public List<DeviceConfig> DeviceConfig = [];


        public DeviceViewModel(IDialogService dialogService,
                               IEventAggregator eventAggregator,
                               IDeviceCommunicationService deviceCommunicationService,
                               IAcquisitionService acquisitionService,
                               IDeviceStorageService deviceStorageService,
                               IS7CommunicationService s7CommunicationService)
        {
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;

            _deviceComunicationService = deviceCommunicationService;
            _acquisitionService = acquisitionService;
            _deviceStorageService = deviceStorageService;

            _s7CommunicationService = s7CommunicationService;

            LoadDeviceCmd = new(async () => await LoadDeviceJson());

            OpenAddCmd = new(() =>
            {
                DialogParameters keyValuePairs = new() 
                { 
                    { "DeviceConfigModels", DeviceConfig },
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

            //UpdataCommand = new(deviceitem => 
            //{
            //    if (deviceitem.ConfigModel.IpAddress != null && _deviceComunicationService.IsConnected(deviceitem.ConfigModel.IpAddress))
            //    {
            //        MessageBox.Show("设备正在连接！请断开连接后重试");
            //        return;
            //    }

            //    DialogParameters keyValuePairs = new()
            //    {
            //        { "DeviceConfigModels", DeviceConfig },
            //        { "DeviceConfigModel", deviceitem.ConfigModel },
            //        { "Mode", "Update" }
            //    };

            //    _dialogService.ShowDialog("DeviceAddWindow", keyValuePairs, async result =>
            //    {
            //        if (result.Result == ButtonResult.OK)
            //        {
            //            await LoadDeviceJson();
            //        }
            //    });

            //});

            DeleteCommand = new(async deviceitem => await DeleteDevice(deviceitem));

            PLCConnectCmd = new(() => {
                s7CommunicationService.S7Connect();
            });
        }


        public async Task LoadDeviceJson()
        {
            Devices.Clear();
            DeviceConfig = await _deviceStorageService.GetDeviceListAsync();

            foreach (var configmodel in DeviceConfig)
            {
                Devices.Add(new DeviceItemViewModel(configmodel, _deviceComunicationService, _acquisitionService));
            }
        }

        
        
        public async Task DeleteDevice(DeviceItemViewModel deviceitem)
        {
            //if (deviceitem.ConfigModel.IpAddress != null && _deviceComunicationService.IsConnected(deviceitem.ConfigModel.IpAddress))
            //{
            //    MessageBox.Show("设备正在连接！请断开连接后重试");
            //    return;
            //}
            Devices.Remove(deviceitem);
            DeviceConfig.Remove(deviceitem.ConfigModel);
            await _deviceStorageService.SaveDeviceListAsync(DeviceConfig);

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
