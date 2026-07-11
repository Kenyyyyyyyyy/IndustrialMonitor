using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Communication.Services;
using IndustrialMonitor.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Formats.Asn1;
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
        public DelegateCommand<string> OpenDialogCmd { get; }

        public DashboardViewModel(IDialogService dialogService, IDeviceCommunicationService deviceCommunicationService)
        {
            _dialogService = dialogService;
            _deviceCommunicationService = deviceCommunicationService;

            OpenDialogCmd = new DelegateCommand<string>(ipAddress =>
            {
                DialogParameters keyValuePairs = new(){{ "IpAddress", ipAddress }};

                _dialogService.ShowDialog("DashboardWindow", keyValuePairs);
            });
        }

        public void GetIpAsync()
        {
            Conlist = _deviceCommunicationService.ScanipList();
        }

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            GetIpAsync();
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


        private ObservableCollection<string> _conList;

        public ObservableCollection<string> Conlist
        {
            get { return _conList; }
            set { _conList = value; }
        }


        #endregion
    }
}
