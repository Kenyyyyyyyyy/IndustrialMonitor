using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.EventSupport.IServices
{
    public interface IEventPublishService
    {
        public void PublishErrorInfo(Guid deviceid, Exception exception);
    }
}
