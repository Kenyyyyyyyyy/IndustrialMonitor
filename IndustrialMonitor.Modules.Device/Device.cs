using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Communication.Services;
using IndustrialMonitor.Core.IRepository;
using IndustrialMonitor.Core.Models;
using IndustrialMonitor.Core.Repository;
using IndustrialMonitor.DataAcquisition.IServices;
using IndustrialMonitor.DataAcquisition.Services;
using IndustrialMonitor.Modules.Device.ViewModels;
using IndustrialMonitor.Modules.Device.Views;
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
            containerRegistry.RegisterForNavigation<DeviceView,DeviceViewModel>();
            containerRegistry.RegisterDialog<DeviceAddWindow,DeviceAddWindowViewModel>();

            containerRegistry.RegisterSingleton<IDeviceCommunicationService, DeviceCommunicationService>();
            containerRegistry.RegisterSingleton<IAcquisitionService, AcquisitionService>();
            containerRegistry.RegisterSingleton<IBaseRepository<DeviceDataModel>,BaseReposity>();
        }
    }
}
