using IndustrialMonitor.Core.GraphModels;
using IndustrialMonitor.DataAcquisition.IServices;
using IndustrialMonitor.DataAcquisition.Services;
using IndustrialMonitor.Modules.Device.Tools;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace IndustrialMonitor.Modules.Dashboard.ViewModels
{
    public class LCGraphDialogViewModel : BindableBase, IDialogAware
    {

        public IGraphDataService _graphDataService;
        private IDeviceStorageService _deviceStorageService;

        public DelegateCommand<string> ChangeValuesCommand { get; set; }

        

        Dictionary<string, List<GraphDataModel>> ydatas = [];

        public string Title { get; } = "数据分析";

        public LCGraphDialogViewModel(IGraphDataService graphDataService, IDeviceStorageService deviceStorageService)
        {
            _graphDataService = graphDataService;
            _deviceStorageService = deviceStorageService;

            ChangeValuesCommand = new DelegateCommand<string>(async (Parameter) => await ChangeValues(Parameter));

        }

        #region IDialogAware
        public DialogCloseListener RequestClose { get; }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            if (ydatas != null) ydatas.Clear();
            Series = [];
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            IpAddress = parameters.GetValue<string>("ipAddress");
            _ = getDeviceId(IpAddress);

            
        }

        public async Task getDeviceId(string ipAddress)
        {
            DeviceId = await _deviceStorageService.GetDeviceIdAsync(ipAddress);
            if (ydatas == null || ydatas.Count == 0) _ = ChangeValues("temputer");
        }

        #endregion

        #region GraphData

        private async Task ChangeValues(string parameter)
        {
            if (ydatas.Count != 0) ydatas.Clear();
            XAxes = [];
            YAxes = [];
            ydatas = new Dictionary<string, List<GraphDataModel>>();
            switch (parameter)
            {
                case "temputer":
                    ydatas = await _graphDataService.GetGraphDataAsync(DeviceId, GraphInterval.Hour, new Dictionary<string, string>
                        {{ "模具温度", "value05" },
                        { "料筒一段温度" , "value06" },
                        { "料筒二段温度" , "value07" },
                        { "喷嘴温度" , "value08" },
                        { "设定模具温度" , "value09" }});
                    break;

                case "stress":

                    ydatas = await _graphDataService.GetGraphDataAsync(DeviceId, GraphInterval.Hour, new Dictionary<string, string>
                        {{ "注射压力", "value10" },
                        { "保压压力" , "value11" },
                        { "锁模力" , "value12" },
                        { "注射速度" , "value13" },
                        { "螺杆转速" , "value14" }});
                    break;

                case "yield":

                    ydatas = await _graphDataService.GetGraphDataAsync
                        (DeviceId, GraphInterval.Hour, new Dictionary<string, string>
                        {{ "今日产量", "value15" },
                        { "良品数量" , "value16" },
                        { "不良品数量" , "value17" },
                        { "良品率" , "value18" },
                        { "当前班次产量" , "value19" }});
                    break;

                case "status":

                    ydatas = await _graphDataService.GetGraphDataAsync
                        (DeviceId, GraphInterval.Hour, new Dictionary<string, string>
                        {{ "当前循环周期", "value20" },
                        { "标准循环周期" , "value21" },
                        { "运行时间" , "value22" },
                        { "停机时间" , "value23" },
                        { "稼动率" , "value24" }});
                    break;

                default:
                    break;
            }

            LoadChart(ydatas);
        }

        public void LoadChart(Dictionary<string, List<GraphDataModel>> ydatas)
        {

            Series = new ISeries[ydatas.Count];
            var colors = new[]
            {
                SKColors.Blue,
                SKColors.Red,
                SKColors.Green,
                SKColors.Orange,
                SKColors.Purple
            };

            int index = 0;

            Series = ydatas.Select(item =>
            {
                var color = colors[index % colors.Length];

                var series = new LineSeries<double>
                {
                    Name = item.Key,
                    Values = item.Value.Select(data => data.value).ToArray(),
                    Stroke = new SolidColorPaint(color, 2),
                    GeometryStroke = new SolidColorPaint(color, 2),
                    GeometryFill = new SolidColorPaint(SKColors.White),
                    GeometrySize = 8,
                    LineSmoothness = 1,
                    ScalesYAt = index,
                };
                index++;

                return series;
            }).ToArray();



            List<string> xlables = ydatas.FirstOrDefault().Value.Select(data => data.dateTime.ToString("HH:mm:ss")).ToList();

            XAxes =
            [
                new Axis
                {
                    Labels = xlables,
                    LabelsRotation = 15,
                    TextSize = 12,
                    Name = "时间",
                    NameTextSize = 14,
                    NamePadding = new LiveChartsCore.Drawing.Padding(0, 10, 0, 0)
                }
            ];

            index = 0;

            YAxes = ydatas.Select(data =>
            {
                var color = colors[index % colors.Length];
                var position = index % 2 == 0 ? AxisPosition.Start : AxisPosition.End;

                var Yaxe = new Axis
                {
                    LabelsPaint = new SolidColorPaint(color, 1),
                    LabelsRotation = 0,
                    TextSize = 10,
                    Position = position,
                };
                index += 1;

                return Yaxe;
            }).ToArray();
        }

        #region propfulls

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

        #endregion

        

        private string _ipAddress;

        public string IpAddress
        {
            get => _ipAddress;
            set => SetProperty(ref _ipAddress, value);
        }
    }
}
