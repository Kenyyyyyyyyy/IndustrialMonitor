using IndustrialMonitor.Alarm.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.EventSupport.Events
{
    public class CommunicationFailedEvent : PubSubEvent<ErrorInfo>
    {
        
    }
}
