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
        

        public DeviceViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            LoadDeviceCmd = new(async () => await LoadDevice());
            DeleteDeviceCmd = new(async deviceConfigModel => await DeleteDevice(deviceConfigModel));

            
            OpenAddCmd = new(() => {

                DialogParameters keyValuePairs = new() { { "DeviceObservableCollection", DeviceObservableCollection } };
                _dialogService.ShowDialog("DeviceAddWindow",keyValuePairs);
            });
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
            
        }

        #endregion
    }
}
