using IndustrialMonitor.Alarm.Models;
using IndustrialMonitor.EventSupport.Events;
using IndustrialMonitor.EventSupport.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.EventSupport.Services
{
    public class EventPublishService : IEventPublishService
    {
        private readonly IEventAggregator _eventAggregator;

        public EventPublishService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void PublishErrorInfo(Guid deviceid, Exception exception) 
        {
            _eventAggregator
                .GetEvent<CommunicationFailedEvent>()
                .Publish(new ErrorInfo
                {
                    deviceid = deviceid,
                    exception = exception
                });
        }
    }
}
