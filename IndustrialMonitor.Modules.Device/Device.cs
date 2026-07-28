using IndustrialMonitor.Alarm.Models;
using IndustrialMonitor.Alarm.Repository;
using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Communication.Services;
using IndustrialMonitor.Core.IRepository;
using IndustrialMonitor.Core.Models;
using IndustrialMonitor.Core.Repository;
using IndustrialMonitor.DataAcquisition.IServices;
using IndustrialMonitor.DataAcquisition.Services;
using IndustrialMonitor.EventSupport.IServices;
using IndustrialMonitor.EventSupport.Services;
using IndustrialMonitor.Modules.Device.Tools;
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
        

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<DeviceView,DeviceViewModel>();
            containerRegistry.RegisterDialog<DeviceAddWindow,DeviceAddWindowViewModel>();

            containerRegistry.RegisterSingleton<IDeviceCommunicationService, DeviceCommunicationService>();
            containerRegistry.RegisterSingleton<IModbusSimulationService,ModbusSimulationService>();
            containerRegistry.RegisterSingleton<IS7CommunicationService, S7CommunicationService>();

            containerRegistry.RegisterSingleton<IEventPublishService, EventPublishService>();

            containerRegistry.RegisterSingleton<IAlarmRecordService, AlarmRecordService>();
            containerRegistry.RegisterSingleton<IndustrialMonitor.Alarm.IRepository.IBaseRepository<AlarmRecord>,
                                                IndustrialMonitor.Alarm.Repository.BaseRepository<AlarmRecord>>();

            containerRegistry.RegisterSingleton<IAcquisitionService, AcquisitionService>();
            containerRegistry.RegisterSingleton<IndustrialMonitor.Core.IRepository.IBaseRepository<DeviceDataModel>,
                                                IndustrialMonitor.Core.Repository.BaseReposity>();
        }
        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<IAlarmRecordService>();
        }

    }
}
