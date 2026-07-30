using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Communication.Services;
using IndustrialMonitor.Core.Models;
using IndustrialMonitor.Modules.Device.DeviceViewModels;
using Prism.Common;
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
        private readonly IDeviceStorageService _deviceStorageService;

        private string _title = "添加设备";

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        

        
        public DialogCloseListener RequestClose { get; }

        public DelegateCommand<string> ControlDeviceCmd { get; }

        public DeviceConfigModel OriginalDeviceConfig { get; set; }

        public DeviceAddWindowViewModel(IDeviceStorageService deviceStorageService)
        {
            _deviceStorageService = deviceStorageService;

            
        }

        

        #region IDialogAware
        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            DeviceConfig = new();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }

        #endregion



        private List<DeviceConfigModel> _deviceConfigList;
        public List<DeviceConfigModel> DeviceConfigList
        {
            get => _deviceConfigList;
            set => SetProperty(ref _deviceConfigList, value);
        }

        private DeviceConfigModel _deviceConfig = new();

        public DeviceConfigModel DeviceConfig
        {
            get => _deviceConfig;
            set => SetProperty(ref _deviceConfig, value);
        }


        public enum DeviceType
        {
            ModbusTcp,
            ModbusRtu,
            SiemensS7
        }

        private DeviceType? _selectedDeviceType;

        public DeviceType? SelectedDeviceType
        {
            get => _selectedDeviceType;
            set 
            {
                if (SetProperty(ref _selectedDeviceType, value))
                {
                    CurrentConfig = value switch
                    {
                        DeviceType.ModbusTcp => new ModbusTcpViewModel(_deviceStorageService),
                        DeviceType.ModbusRtu => new ModbusRtuViewModel(),
                        DeviceType.SiemensS7 => new S7ViewModel(),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    
                }
            }
        }

        private object _currentConfig;

        public object CurrentConfig
        {
            get => _currentConfig;
            set => SetProperty(ref _currentConfig, value);
        }


        public ObservableCollection<DeviceType> DeviceTypes { get; } = new(Enum.GetValues<DeviceType>());


    }
}
