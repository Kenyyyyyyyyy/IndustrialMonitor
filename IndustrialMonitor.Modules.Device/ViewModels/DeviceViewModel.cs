using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Communication.Services;
using IndustrialMonitor.Core.Models;
using IndustrialMonitor.Modules.Device.Tools;
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

        public DelegateCommand LoadDeviceCmd { get; }
        public DelegateCommand OpenAddCmd { get; }

        public DelegateCommand UpdataCommand { get; }
        public DelegateCommand<DeviceConfigModel> DeleteDeviceCmd{ get; }

        public ObservableCollection<DeviceItemViewModel> Devices { get; } = [];
        public List<DeviceConfigModel> DeviceConfigModels = [];


        public DeviceViewModel(IDialogService dialogService,IEventAggregator eventAggregator,IDeviceCommunicationService deviceCommunicationService)
        {
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;
            _deviceComunicationService = deviceCommunicationService;

            LoadDeviceCmd = new(async () => await LoadDeviceJson());

            


            OpenAddCmd = new(() =>
            {
                DialogParameters keyValuePairs = new() { { "DeviceConfigModels", DeviceConfigModels } };
                _dialogService.ShowDialog("DeviceAddWindow", keyValuePairs,async result =>
                {
                    if (result.Result == ButtonResult.OK) 
                    {
                        await LoadDeviceJson();
                    }
                });
            });

            UpdataCommand = new(() => { });

        }


        public async Task LoadDeviceJson()
        {
            Devices.Clear();
            DeviceConfigModels = await _deviceStorageService.LoadDeviceJson();

            foreach (var configmodel in DeviceConfigModels)
            {
                Devices.Add(new DeviceItemViewModel(configmodel, _deviceComunicationService));
            }
        }

        #region 连接 断开 删除


        public async Task DeleteDevice(DeviceItemViewModel device)
        {
            if (_deviceComunicationService.IsConnected(device.ConfigModel.IpAddress))
            {
                MessageBox.Show("设备正在连接！请断开连接后重试");
            }
            Devices.Remove(device);
            DeviceConfigModels.Remove(device.ConfigModel);
            await _deviceStorageService.SaveDeviceAsJsonAsync(DeviceConfigModels);

        }

        #endregion

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
            _ = LoadDeviceJson();
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
