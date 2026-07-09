using IndustrialMonitor.Core.GraphModels;
using IndustrialMonitor.DataAcquisition.IServices;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
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

        Dictionary<string,List<GraphDataModel>> ydatas = [];

        

        public IGraphDataService _graphDataService;
        public DelegateCommand<string> ChangeValuesCommand { get; set; }

        public LCGraphViewModel(IGraphDataService graphDataService)
        {
            _graphDataService = graphDataService;
            ChangeValuesCommand = new DelegateCommand<string>(async (Parameter) => await ChangeValues(Parameter));
        }

        private async Task ChangeValues(string parameter)
        {
            switch (parameter)
            {
                case "temputer":
                    
                    ydatas = await _graphDataService.GetGraphDataAsync
                        (DeviceId, GraphInterval.Hour, ["value05", "value06", "value07", "value08", "value09"]);
                    break;

                case "stress":
                    
                    ydatas = await _graphDataService.GetGraphDataAsync
                        (DeviceId, GraphInterval.Hour, ["value10", "value11", "value12", "value13", "value14"]);
                    break;

                case "yield":
                    
                    ydatas = await _graphDataService.GetGraphDataAsync
                        (DeviceId, GraphInterval.Hour, ["value15", "value16", "value17", "value18", "value19"]);
                    break;

                case "status":
                    
                    ydatas = await _graphDataService.GetGraphDataAsync
                        (DeviceId, GraphInterval.Hour, ["value20", "value21", "value22", "value23", "value24"]);
                    break;

                default:
                    break;
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            DeviceId = navigationContext.Parameters.GetValue<Guid>("DeviceId");

            
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public void LoadChart(Dictionary<string, List<GraphDataModel>> ydatas)
        {
            var colors = new[]
            {
                SKColors.Blue,
                SKColors.Red,
                SKColors.Green,
                SKColors.Orange
            };

            Series = ydatas.Select(item =>
            {
                var series = new LineSeries<ushort>
                {
                    Name = item.Key,
                    Values = item.Value.Select(data => data.value).ToList(),
                };

                return series;
            }).ToArray();
        }

        #region

        private Guid _deviceId;

        public Guid DeviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; }
        }

        private List<int> _registersList;

        public List<int> RegistersList
        {
            get { return _registersList; }
            set { _registersList = value; }
        }




        #endregion


        #region graph data

        private ISeries[] _series;
        public ISeries[] Series
        {
            get => _series;
            set => SetProperty(ref _series, value);
        }

        private Axis[] _xAxes;
        public Axis[] XAxes
        {
            get => _xAxes;
            set => SetProperty(ref _xAxes, value);
        }

        private Axis[] _yAxes;
        public Axis[] YAxes
        {
            get => _yAxes;
            set => SetProperty(ref _yAxes, value);
        }



        #endregion
    }
}
