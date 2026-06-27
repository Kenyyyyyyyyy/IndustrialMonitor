using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Core.Events
{
    public class DeviceConnectedEvent: PubSubEvent<ObservableCollection<string>>
    {

    }
}
