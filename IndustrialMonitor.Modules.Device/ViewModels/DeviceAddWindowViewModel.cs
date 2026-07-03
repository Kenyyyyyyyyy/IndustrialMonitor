using IndustrialMonitor.Core.Models;
using IndustrialMonitor.Modules.Device.Tools;
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

        private string _title;

        public string Title
        {
            get =>  _title; 
            set =>  SetProperty(ref _title, value);
        }

        private string _buttonConCent;

        public string ButtonConCent
        {
            get { return _buttonConCent; }
            set { _buttonConCent = value; }
        }


        public string Mode { get; }

        private readonly DeviceStorageService _deviceStorageService = new();
        public DialogCloseListener RequestClose { get; }

        public DelegateCommand<string> ControlDeviceCmd { get; }

        public DeviceConfigModel originalDeviceConfig { get; set; }

        public DeviceAddWindowViewModel()
        {
            ControlDeviceCmd = new(async (buttoncontent) => 
            {
                if(buttoncontent == "添加")
                {
                    await AddDevice();
                }

                if (buttoncontent == "修改")
                {
                    await UpdateDevice();
                }
            });
        }

        public async Task AddDevice()
        {
            _deviceConfigList.Add(DeviceConfig);
            await _deviceStorageService.SaveDeviceAsJsonAsync(_deviceConfigList);
            RequestClose.Invoke(ButtonResult.OK);
        }

        public async Task UpdateDevice()
        {
            int index = _deviceConfigList.FindIndex(x => x.Id == originalDeviceConfig.Id);

            if (index < 0)
            {
                MessageBox.Show("json的devicelist中找不到此device");
                return;
            }

            _deviceConfigList[index] = DeviceConfig;
            await _deviceStorageService.SaveDeviceAsJsonAsync(_deviceConfigList);
            
            RequestClose.Invoke(ButtonResult.OK);
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
            _deviceConfigList = parameters.GetValue<List<DeviceConfigModel>>("DeviceConfigModels");

            if (parameters.GetValue<string>("Mode") == "Add")
            {
                Title = "添加设备";
                ButtonConCent = "添加";
            }
            if (parameters.GetValue<string>("Mode") == "Update")
            {
                Title = "修改设备";
                ButtonConCent = "修改";
                originalDeviceConfig = parameters.GetValue<DeviceConfigModel>("DeviceConfigModel");
                DeviceConfig = new DeviceConfigModel
                {
                    Id = originalDeviceConfig.Id,
                    IpAddress = originalDeviceConfig.IpAddress,
                    Port = originalDeviceConfig.Port,
                    SlaveId = originalDeviceConfig.SlaveId,
                    StartAddress = originalDeviceConfig.StartAddress,
                    NumberOfPoints = originalDeviceConfig.NumberOfPoints
                };
            }
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
    }
}
