using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndustrialMonitor.Alarm.IRepository;
using IndustrialMonitor.Alarm.Models;
using IndustrialMonitor.DataAcquisition.IServices;
using IndustrialMonitor.EventSupport.IServices;

namespace IndustrialMonitor.DataAcquisition.Services
{
    public class AlarmDataService : IAlarmDataService
    {
        private readonly IBaseRepository<AlarmRecord> _baseRepository;
        private readonly IEventPublishService _eventPublishService;

       


        public AlarmDataService(IBaseRepository<AlarmRecord> baseRepository, IEventPublishService eventPublishService)
        {
            _baseRepository = baseRepository;
            _eventPublishService = eventPublishService;

            
        }

        public async Task<List<AlarmRecord>> GetAlarmRecords(byte pageIndex,byte pageSize)
        {
            try
            {
                var alarmList = await _baseRepository.GetAlarmRecordsAsync(pageIndex, pageSize);
                return alarmList;
            }
            catch (Exception ex)
            {
                _eventPublishService.PublishCRUDErrorInfo(ex);
                return [];
            }
        }
    }
}
