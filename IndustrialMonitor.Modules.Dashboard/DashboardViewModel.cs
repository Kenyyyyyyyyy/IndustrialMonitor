using IndustrialMonitor.Communication.Modbus.TCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IndustrialMonitor.Modules.Dashboard
{
    public class DashboardViewModel : BindableBase , INavigationAware
    {

        private readonly ModBusTCP _modBusTCP = new();
        private CancellationTokenSource? _cts;
        private bool _isCollecting = false;

        public DashboardViewModel()
        {
            
            
        }

        public async Task TCPConnectAsync()
        {
            
            try
            {
                await _modBusTCP.ModbusTcpConnectAsync("127.0.0.1", 5020);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException?.Message ?? ex.Message);
            }
        }


        public async Task ReadregistersAsync()
        {
            if (_cts == null) return;

            CancellationToken cancellationToken = _cts.Token;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    ushort[] registers = await _modBusTCP.ReadHoldingRegistersAsync(1, 0, 10);
                    TodayOutput = registers[0];
                    YieldRate = registers[1];
                    AlarmCount = registers[2];
                    DeviceStatus = registers[6];
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.InnerException?.Message ?? ex.Message);
                }

                await Task.Delay(1000, cancellationToken);
            }

            

        }

        public async Task StartCollectAsync()
        {
            if (_isCollecting) return;

            _isCollecting = true;
            _cts = new CancellationTokenSource();

            try
            {
                await TCPConnectAsync();
                await ReadregistersAsync();
            }
            catch (OperationCanceledException)
            {
            }
            catch
            {
                StopCollectAsync();
            }
        }

        public void StopCollectAsync()
        {
            if (!_isCollecting) return;

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;


            _isCollecting = false;
        }


        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _ = StartCollectAsync();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            StopCollectAsync();
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
