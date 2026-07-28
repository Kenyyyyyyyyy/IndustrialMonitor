using IndustrialMonitor.Alarm.Models;
using IndustrialMonitor.Core.GraphModels;
using IndustrialMonitor.Core.IRepository;
using IndustrialMonitor.DataAcquisition.IServices;
using IndustrialMonitor.EventSupport.IServices;
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
        private IEventPublishService _eventPublishService;

        public Dictionary<string, List<GraphDataModel>> RasiterDateValuePairs = [];

        public GraphDataService(IGraphDataRepository graphDataRepository,
                                IEventPublishService eventPublishService)
        {
            _graphDataRepository = graphDataRepository;
            _eventPublishService = eventPublishService;
        }

        public async Task<Dictionary<string, List<GraphDataModel>>> GetGraphDataAsync
            (Guid DeviceId, GraphInterval Interval, Dictionary<string,string> RegisterAddress)
        {
            
            var tasks = RegisterAddress.Select(async registerAddress => 
            {
                GraphRequestModel graphRequestModel = new()
                {
                    DeviceId = DeviceId,
                    Interval = Interval,
                    RegisterAddress = registerAddress.Value
                };

                try
                {
                    List<GraphDataModel> graphDatas = await _graphDataRepository.GetDataByDeviceIdAndInterval(graphRequestModel);
                    RasiterDateValuePairs[registerAddress.Key] = graphDatas;
                }
                catch (Exception ex) 
                {
                    _eventPublishService.PublishErrorInfo(DeviceId,ex);
                }
            });

            await Task.WhenAll(tasks);

            return RasiterDateValuePairs;

            
    }
    }
}
