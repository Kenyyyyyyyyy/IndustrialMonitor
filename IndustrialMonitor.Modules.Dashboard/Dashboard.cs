using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Communication.Services;
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

            containerRegistry.RegisterSingleton<IDeviceCommunicationService,DeviceCommunicationService>();
        }
    }
}
