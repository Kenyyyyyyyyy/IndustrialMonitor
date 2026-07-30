using IndustrialMonitor.Alarm.IRepository;
using IndustrialMonitor.Alarm.Models;
using IndustrialMonitor.Alarm.Repository;
using IndustrialMonitor.DataAcquisition.IServices;
using IndustrialMonitor.DataAcquisition.Services;
using IndustrialMonitor.Modules.Monitor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Modules.Monitor
{
    public class Monitor : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<IndustrialMonitor.Modules.Monitor.MonitorView, MonitorViewModel>();

            containerRegistry.RegisterSingleton<IAlarmDataService, AlarmDataService>();
            
        }
    }
}
