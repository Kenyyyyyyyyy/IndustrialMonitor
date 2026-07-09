using IndustrialMonitor.Core.GraphModels;
using IndustrialMonitor.Core.IRepository;
using IndustrialMonitor.DataAcquisition.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.DataAcquisition.Services
{
    public class GraphDataService : IGraphDataService
    {
        private IGraphDataRepository _graphDataRepository;

        public Dictionary<string, List<GraphDataModel>> RasiterDateValuePairs = [];

        public GraphDataService(IGraphDataRepository graphDataRepository)
        {
            _graphDataRepository = graphDataRepository;
        }

        public async Task<Dictionary<string, List<GraphDataModel>>> GetGraphDataAsync(Guid DeviceId, GraphInterval Interval, List<string> RegisterAddress)
        {
            
            var tasks = RegisterAddress.Select(async registerAddress => 
            {
                GraphRequestModel graphRequestModel = new()
                {
                    DeviceId = DeviceId,
                    Interval = Interval,
                    RegisterAddress = registerAddress
                };

                List<GraphDataModel> graphDatas = await _graphDataRepository.GetDataByDeviceIdAndInterval(graphRequestModel);
                RasiterDateValuePairs[registerAddress] = graphDatas;
            });

            await Task.WhenAll(tasks);

            return RasiterDateValuePairs;

        }
    }
}
