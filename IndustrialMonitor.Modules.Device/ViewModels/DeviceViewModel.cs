using IndustrialMonitor.Core.Models;
using IndustrialMonitor.Modules.Device.Tools;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Modules.Device.ViewModels
{
    public class DeviceViewModel : BindableBase ,INavigationAware
    {

        private readonly DeviceStorageService _deviceStorageService = new();
        private readonly IDialogService _dialogService;
        public DelegateCommand LoadDeviceCmd { get; }
        public DelegateCommand OpenAddCmd { get; }
        public DelegateCommand<DeviceConfigModel> DeleteDeviceCmd{ get; }
        public DelegateCommand<DeviceConfigModel> AddDeviceCmd { get; }

        public DeviceViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            LoadDeviceCmd = new(async () => await LoadDevice());
            DeleteDeviceCmd = new(async deviceConfigModel => await DeleteDevice(deviceConfigModel));
            AddDeviceCmd = new(async deviceConfigModel => await AddDevice(deviceConfigModel));
            OpenAddCmd = new(() => _dialogService.ShowDialog("DeviceAddWindow"));
        }

        private ObservableCollection<DeviceConfigModel> _deviceObservableCollection;
        public ObservableCollection<DeviceConfigModel> DeviceObservableCollection
        {
            get => _deviceObservableCollection;
            set => SetProperty(ref _deviceObservableCollection, value);
        }


        public async Task LoadDevice()
        {
            if(DeviceObservableCollection != null) DeviceObservableCollection.Clear();
            DeviceObservableCollection = await _deviceStorageService.LoadDeviceJson();
        }

        public async Task DeleteDevice(DeviceConfigModel deviceConfigModel)
        {
            DeviceObservableCollection.Remove(deviceConfigModel);
            await _deviceStorageService.SaveDeviceAsJsonAsync(DeviceObservableCollection);
        }

        public async Task AddDevice(DeviceConfigModel deviceConfigModel)
        {
            DeviceObservableCollection.Add(deviceConfigModel);
            await _deviceStorageService.SaveDeviceAsJsonAsync(DeviceObservableCollection);
        }


        private DeviceConfigModel _device = new();

        public DeviceConfigModel Device
        {
            get => _device;
            set => SetProperty(ref _device, value);
        }

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //读json 
            _ = LoadDevice();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
