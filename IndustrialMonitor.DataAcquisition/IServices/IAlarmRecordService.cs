using IndustrialMonitor.Alarm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.DataAcquisition.IServices
{
    public interface IAlarmRecordService
    {
        public void RecordAlarmAsync(ErrorInfo errorInfo);
    }
}
