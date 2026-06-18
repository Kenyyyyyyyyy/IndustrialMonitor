using IndustrialMonitor.Communication.Modbus.TCP;
using IndustrialMonitor.Core.Models;
using Prism.Navigation.Regions;
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
    public class DashboardWindowViewModel : BindableBase, IDialogAware
    {
        public string Title { get; } = "设备详情";

        private CancellationTokenSource? _cts;  
        bool isCollecting = false;
        DetailHelper detailHelper = new();

        public ObservableCollection<DeviceDetailModel> DeviceDetails { get; set; } = [];
        
        
        private ObservableCollection<DeviceDetailModel> _temperatureItems;

        public ObservableCollection<DeviceDetailModel> TemperatureItems
        {
            get { return _temperatureItems; }
            set => SetProperty(ref _temperatureItems, value);
        }

        private ObservableCollection<DeviceDetailModel> _processItems;

        public ObservableCollection<DeviceDetailModel> ProcessItems
        {
            get { return _processItems; }
            set => SetProperty(ref _processItems, value);
        }

        private ObservableCollection<DeviceDetailModel> _productionItems;

        public ObservableCollection<DeviceDetailModel> ProductionItems
        {
            get { return _productionItems; }
            set => SetProperty(ref _productionItems, value);
        }

        private ObservableCollection<DeviceDetailModel> _runningItems;

        public ObservableCollection<DeviceDetailModel> RunningItems
        {
            get { return _runningItems; }
            set => SetProperty(ref _runningItems, value);
        }


        TcpClient _tcpClient = new();
        private readonly ModBusTCP _modBusTCP = new();

        public DialogCloseListener RequestClose { get; }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            StopCollectAsync();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _cts = new CancellationTokenSource();

            _tcpClient = parameters.GetValue<TcpClient>("tcpClients");
            IpAddress = parameters.GetValue<string>("IpAddress");
            DeviceName = parameters.GetValue<string>("IpAddress");
            detailHelper.InitDeviceDetailItems();
            
            _ = StartCollectAsync();
            
        }



        public async Task ReadregistersAsync()
        {
            if (_cts == null) return;

            CancellationToken cancellationToken = _cts.Token;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    detailHelper.UpdateDeviceDetails(await _modBusTCP.ReadHoldingRegistersAsync(1, 0, 30, _tcpClient));

                    DeviceDetails = detailHelper.DeviceDetails;
                    TemperatureItems = detailHelper.TemperatureItems;
                    ProcessItems = detailHelper.ProcessItems;
                    ProductionItems = detailHelper.ProcessItems;
                    RunningItems = detailHelper.RunningItems;

                    DeviceStatus = DeviceDetails[0].Value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.InnerException?.Message ?? ex.Message);
                }

                await Task.Delay(1000, cancellationToken);
            }
        }

        private async Task StartCollectAsync()
        {
            if(isCollecting) return;

            isCollecting = true;
            await ReadregistersAsync();
        }

        private void StopCollectAsync()
        {
            if (!isCollecting) return;

            _cts?.Cancel();
            _cts?.Dispose();

            isCollecting = false;
        }


        private string _deviceName;
        public string DeviceName
        {
            get => _deviceName; 
            set => SetProperty(ref _deviceName, value);
        }

        private string _ipAddress;

        public string IpAddress
        {
            get => _ipAddress;
            set => SetProperty(ref _ipAddress, value);
        }

        private string _deviceStatus;

        public string DeviceStatus
        {
            get => _deviceStatus;
            set => SetProperty(ref _deviceStatus, value);
        }
    }
}
