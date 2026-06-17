using IndustrialMonitor.Communication.Modbus.TCP;
using IndustrialMonitor.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IndustrialMonitor.Modules.Dashboard
{
    public class DashboardWindowViewModel : BindableBase, INavigationAware
    {

        CancellationTokenSource _cts;  
        bool isCollecting = false;

        public ObservableCollection<DeviceDetailModel> DeviceDetails { get; set; } = [];
        public ObservableCollection<DeviceDetailModel> TemperatureItems { get; } = new();
        public ObservableCollection<DeviceDetailModel> ProcessItems { get; } = new();
        public ObservableCollection<DeviceDetailModel> ProductionItems { get; } = new();
        public ObservableCollection<DeviceDetailModel> RunningItems { get; } = new();

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            StopCollect();
        }

        
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if(navigationContext.Parameters.ContainsKey("modbusClients"))
            {
                Dictionary<string, ModBusTCP> modbusClients = navigationContext.Parameters.GetValue<Dictionary<string, ModBusTCP>>("modbusClients");
            }
            if(!isCollecting)
            {
                StartCollect();
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
                    //ushort[] registers = await _modBusTCP.ReadHoldingRegistersAsync(1, 0, 30);
                    //TodayOutput = registers[0];
                    //YieldRate = registers[1];
                    //AlarmCount = registers[2];
                    //DeviceStatus = registers[6];
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.InnerException?.Message ?? ex.Message);
                }

                await Task.Delay(1000, cancellationToken);
            }
        }

        private void StartCollect()
        {
            if(isCollecting) return;
            isCollecting = true;
        }

        private void StopCollect()
        {
            throw new NotImplementedException();
        }


        

    }
}
