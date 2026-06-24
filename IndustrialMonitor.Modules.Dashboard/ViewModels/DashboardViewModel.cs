using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Communication.Services;
using IndustrialMonitor.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IndustrialMonitor.Modules.Dashboard.ViewModels
{
    public class DashboardViewModel : BindableBase, INavigationAware
    {
        private readonly IDialogService _dialogService;
        private readonly IDeviceCommunicationService _deviceCommunicationService;
        public DelegateCommand<string> OpenDetailCmd { get; }



        public DashboardViewModel(IDialogService dialogService, IDeviceCommunicationService deviceCommunicationService)
        {
            _dialogService = dialogService;
            _deviceCommunicationService = deviceCommunicationService;

            OpenDetailCmd = new DelegateCommand<string>(ipAddress =>
            {
                DialogParameters keyValuePairs = new(){{ "IpAddress", ipAddress }};
                _dialogService.ShowDialog("DashboardWindow", keyValuePairs);
            });
        }

        public async Task StartConnectAsync()
        {
            DeviceConnectionResult = await _deviceCommunicationService.ScanipList(_devicepairs);
        }

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _ = StartConnectAsync();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        #endregion




        #region allview
        private int _todayOutPut;

        public int TodayOutput
        {
            get => _todayOutPut; 
            set => SetProperty(ref _todayOutPut, value); 
        }

        private int _yieldRate;

        public int YieldRate
        {
            get => _yieldRate;
            set => SetProperty(ref _yieldRate, value);
        }

        private int _alarmCount;

        public int AlarmCount
        {
            get => _alarmCount;
            set => SetProperty(ref _alarmCount, value);
        }

        private int _deviceStatus;

        public int DeviceStatus
        {
            get => _deviceStatus;
            set => SetProperty(ref _deviceStatus, value);
        }

        

        private ObservableCollection<DeviceConnectionResult> _deviceConnectionResult;

        public ObservableCollection<DeviceConnectionResult> DeviceConnectionResult
        {
            get => _deviceConnectionResult;
            set => SetProperty(ref _deviceConnectionResult, value);
        }

        //private List<string> _scanList = ["127.0.0.1", "127.0.0.2", "127.0.0.3", "127.0.0.4"];

        //public List<string> ScanList
        //{
        //    get => _scanList;
        //    set => SetProperty(ref _scanList, value);
        //}

        private readonly Dictionary<string, int> _devicepairs = new Dictionary<string, int>
        {
            {"127.0.0.1",502 },
            {"127.0.0.2",502 },
            {"127.0.0.3",502 },
            {"127.0.0.4",502 }
        };

        #endregion
    }
}
