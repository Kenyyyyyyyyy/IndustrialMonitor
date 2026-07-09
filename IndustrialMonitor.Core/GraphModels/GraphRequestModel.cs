using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Core.GraphModels
{
    public class GraphRequestModel
    {
        public Guid DeviceId { get; set; }
        public GraphInterval Interval { get; set; }
        public string? RegisterAddress { get; set; }
    }
}
