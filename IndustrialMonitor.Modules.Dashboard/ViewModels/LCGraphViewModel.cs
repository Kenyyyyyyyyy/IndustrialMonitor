using IndustrialMonitor.DataAcquisition.IServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Modules.Dashboard.ViewModels
{
    public class LCGraphViewModel: BindableBase, INavigationAware
    {
        public double[] Values1 { get; set; } = [14, 13, 14, 15, 17];

        public double[] Values2 { get; set; } = [10, 11, 12, 13, 14];

        public double[] Values3 { get; set; } = [14, 13, 12, 11, 17];

        public double[] Values4 { get; set; } = [7, 13, 6, 15, 5];

        Dictionary<string,List<ushort>> ydatas = [];

        public IGraphDataService _graphDataService;
        public DelegateCommand<string> ChangeValuesCommand { get; set; }

        public LCGraphViewModel(IGraphDataService graphDataService)
        {
            _graphDataService = graphDataService;
            ChangeValuesCommand = new DelegateCommand<string>(ChangeValues);

            
        }

        private void ChangeValues(string parameter)
        {
            switch (parameter)
            {
                case "temputer":
                    //ydatas = _graphDataService.GetGraphDataAsync("DeviceId", "IntervalTime", new List<string> { "RegisterAddress1" }).Result;
                    break;
                case "stress":
                    Values2 = [10, 11, 12, 13, 14];
                    break;
                case "yield":
                    Values3 = [14, 13, 12, 11, 17];
                    break;
                case "status":
                    Values4 = [7, 13, 6, 15, 5];
                    break;
                default:
                    break;
            }
        }   
    }
}
