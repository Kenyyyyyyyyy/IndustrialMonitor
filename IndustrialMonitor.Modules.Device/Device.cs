using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Modules.Device
{
    public class Device : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
             
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<DeviceView>();
        }
    }
}
