using IndustrialMonitor.Communication.Modbus.TCP;
using IndustrialMonitor.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IndustrialMonitor.Modules.Dashboard
{
    public class DashboardViewModel : BindableBase, INavigationAware
    {
        private IDialogService _dialogService;

        private List<string> _scanList = ["127.0.0.1", "127.0.0.2", "127.0.0.3", "127.0.0.4"];
        
        public DelegateCommand<TcpResultModel> OpenDetailCmd { get; }



        public DashboardViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            OpenDetailCmd = new DelegateCommand<TcpResultModel>(TcpResult =>
            {
            if (TcpResult == null || TcpResult.IsConnected == false || TcpResult.TcpClient == null) return;

                DialogParameters para = [];
                para.Add("tcpClients", TcpResult.TcpClient);
                para.Add("IpAddress", TcpResult.IpAddress);



                _dialogService.ShowDialog("DashboardWindow", para);
            });


        }

        public async Task StartConnectAsync()
        {
            TcpScanFunc tcpScanFunc = new();
            TcpResult = await tcpScanFunc.TCPConnect(_scanList);
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

        

        private ObservableCollection<TcpResultModel> _tcpResults;

        public ObservableCollection<TcpResultModel> TcpResult
        {
            get => _tcpResults;
            set => SetProperty(ref _tcpResults, value);
        }


        #endregion
    }
}
