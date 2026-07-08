using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.DataAcquisition.IServices
{
    public interface IGraphDataService
    {
        public Task<List<ushort>> GetGraphDataAsync(string DeviceId, string IntervalTime,List<string> RegisterAddress);
    }
}
