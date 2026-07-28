using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Alarm.Models
{
    public class ErrorInfo
    {
        public Guid deviceid;
        public Exception exception;
    }
}
