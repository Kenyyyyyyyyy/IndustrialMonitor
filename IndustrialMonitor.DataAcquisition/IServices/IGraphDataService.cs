using IndustrialMonitor.Core.GraphModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.DataAcquisition.IServices
{
    public interface IGraphDataService
    {
        public Task<Dictionary<string, List<GraphDataModel>>> GetGraphDataAsync(Guid DeviceId, GraphInterval Interval, List<string> RegisterAddress);
        
    }
}
