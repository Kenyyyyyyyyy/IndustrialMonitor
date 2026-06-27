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

        #endregion
    }
}
