using IndustrialMonitor.Communication.Modbus.TCP;
using System;
using System.Collections.Generic;
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
        private readonly ModBusTCP _modBusTCP = new();
        private bool _isCollecting = false;
        private List<string> _connectList = [ "127.0.0.1", "127.0.0.2", "127.0.0.3", "127.0.0.4" ];
        public List<string> ConnectList { get; set; } = [];
        private readonly Dictionary<string, TcpClient> _modbusClients = new();
        public DelegateCommand<string> OpenDetailCmd { get; }

        TcpClient _tcpClient = new();

        public DashboardViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            OpenDetailCmd = new DelegateCommand<string>(ip =>
            {
                DialogParameters para = [];
                para.Add("tcpClients", _modbusClients[ip]);
                para.Add("IpAddress",ip);
                _dialogService.ShowDialog("DashboardWindow",para);
            });


        }

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _ = TCPConnectAsync();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("modbusClients", _modbusClients);
        }

        #endregion

        public async Task TCPConnectAsync()
        {
            _modbusClients.Clear();
            

            foreach (var ip in _connectList)
            {
                try
                {
                    _tcpClient = await _modBusTCP.ModbusTcpConnectAsync(ip, 502);
                    _modbusClients.Add(ip, _tcpClient);
                    ConnectList.Add(ip);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"无法连接到 {ip}: {ex.InnerException?.Message ?? ex.Message}");
                }
            }

        }



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
