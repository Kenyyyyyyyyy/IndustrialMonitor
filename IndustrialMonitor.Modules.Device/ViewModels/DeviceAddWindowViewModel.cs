using IndustrialMonitor.Core.Models;
using IndustrialMonitor.Modules.Device.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

            DeviceConfigModels.Add(Device);
            await _deviceStorageService.SaveDeviceAsJsonAsync(DeviceConfigModels);
            Device = new();
            RequestClose.Invoke(ButtonResult.OK);
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
            DeviceConfigModels = parameters.GetValue<List<DeviceConfigModel>>("DeviceConfigModels");
        }

        #endregion



        private List<DeviceConfigModel> _deviceConfigModels;
        public List<DeviceConfigModel> DeviceConfigModels
        {
            get => _deviceConfigModels;
            set => SetProperty(ref _deviceConfigModels, value);
        }

        private DeviceConfigModel _device = new();

        public DeviceConfigModel Device
        {
            get => _device;
            set => SetProperty(ref _device, value);
        }
    }
}
