using IndustrialMonitor.Alarm.Models;
using IndustrialMonitor.Core.GraphModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Core.IRepository
{
    public interface IGraphDataRepository
    {
        public Task<List<GraphDataModel>> GetDataByDeviceIdAndInterval(GraphRequestModel graphRequestModel);

        
    }
}
