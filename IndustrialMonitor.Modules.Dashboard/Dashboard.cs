using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Communication.Services;
using IndustrialMonitor.DataAcquisition.IServices;
using IndustrialMonitor.DataAcquisition.Services;
using IndustrialMonitor.Modules.Dashboard.ViewModels;
using IndustrialMonitor.Modules.Dashboard.Views;
using IndustrialMonitor.Modules.Device.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Modules.Dashboard
{
    public partial class Dashboard : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<DashboardView, DashboardViewModel>("DashboardView");

            containerRegistry.RegisterDialog<DashboardWindow,DashboardWindowViewModel>("DashboardWindow");

            containerRegistry.RegisterForNavigation<LCGraphUserControl, LCGraphViewModel>("LCGraphUserControl");

            containerRegistry.RegisterSingleton<IDeviceCommunicationService,DeviceCommunicationService>();

            containerRegistry.RegisterSingleton<IDeviceStorageService, DeviceStorageService>();

            containerRegistry.RegisterSingleton<IGraphDataService,GraphDataService>();
        }
    }
}
