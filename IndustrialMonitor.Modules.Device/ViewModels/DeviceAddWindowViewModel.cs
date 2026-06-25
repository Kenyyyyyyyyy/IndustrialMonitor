using IndustrialMonitor.Core.Models;
using IndustrialMonitor.Modules.Device.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Modules.Device.ViewModels
{
    public class DeviceAddWindowViewModel : BindableBase, IDialogAware
    {
        public string Title { get; } ="添加设备";
        private readonly DeviceStorageService _deviceStorageService = new();
        public DialogCloseListener RequestClose { get; }

        public DelegateCommand AddDeviceCmd { get; }

        public DeviceAddWindowViewModel()
        {
            AddDeviceCmd = new(async () => await AddDevice());
        }

        public async Task AddDevice()
        {
            
            DeviceObservableCollection.Add(Device);
            await _deviceStorageService.SaveDeviceAsJsonAsync(DeviceObservableCollection);
            Device = new();
            
        }

        #region IDialogAware
        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            DeviceObservableCollection = parameters.GetValue<ObservableCollection<DeviceConfigModel>>("DeviceObservableCollection");
        }

        #endregion



        private ObservableCollection<DeviceConfigModel> _deviceObservableCollection;
        public ObservableCollection<DeviceConfigModel> DeviceObservableCollection
        {
            get => _deviceObservableCollection;
            set => SetProperty(ref _deviceObservableCollection, value);
        }

        private DeviceConfigModel _device = new();

        public DeviceConfigModel Device
        {
            get => _device;
            set => SetProperty(ref _device, value);
        }
    }
}
