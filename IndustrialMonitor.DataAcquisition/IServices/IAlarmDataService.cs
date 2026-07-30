using IndustrialMonitor.Alarm.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.DataAcquisition.IServices
{
    public interface IAlarmDataService
    {
        
        public Task<List<AlarmRecord>> GetAlarmRecords(byte pageIndex, byte pageSize);

    }
}
