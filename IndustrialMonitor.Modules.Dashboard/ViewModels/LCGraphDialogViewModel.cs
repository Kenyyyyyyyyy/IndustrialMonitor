using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Core.GraphModels;
using IndustrialMonitor.DataAcquisition.IServices;
using IndustrialMonitor.DataAcquisition.Services;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Windows.Foundation.Metadata;

namespace IndustrialMonitor.Modules.Dashboard.ViewModels
{
    public class LCGraphDialogViewModel : BindableBase, IDialogAware
    {

        public IGraphDataService _graphDataService;
        private IDeviceStorageService _deviceStorageService;

        public DelegateCommand<string> ChangeItemCommand { get; set; }

        public DelegateCommand<string> ChangeIntervalCommand { get; set; }

        

        Dictionary<string, List<GraphDataModel>> ydatas = [];

        

        public string Title { get; } = "数据分析";

        public LCGraphDialogViewModel(IGraphDataService graphDataService, IDeviceStorageService deviceStorageService)
        {
            _graphDataService = graphDataService;
            _deviceStorageService = deviceStorageService;

            ChangeItemCommand = new DelegateCommand<string>(async (parameter) =>
            {
                GraphItem = parameter;
                await ChangeValues();
            });

            ChangeIntervalCommand = new DelegateCommand<string>(async parameter =>
            {
                switch (parameter)
                {
                    case "min":
                        GraphInterval = GraphInterval.Min;
                        await ChangeValues();
                        break;
                    case "hour":
                        GraphInterval = GraphInterval.Hour;
                        await ChangeValues();
                        break;
                    case "day":
                        GraphInterval = GraphInterval.Day;
                        await ChangeValues();
                        break;
                    case "week":
                        GraphInterval = GraphInterval.Week;
                        await ChangeValues();
                        break;
                    case "month":
                        GraphInterval = GraphInterval.Month;
                        await ChangeValues();
                        break;
                }
            });


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
            if (ydatas == null || ydatas.Count == 0)
            {
                GraphItem = "temputer";
                _ = ChangeValues();
            }
        }

        #endregion

        #region GraphData

        private async Task ChangeValues()
        {
            if (ydatas.Count != 0) ydatas.Clear();
            XAxes = [];
            YAxes = [];
            ydatas = new Dictionary<string, List<GraphDataModel>>();
            switch (GraphItem)
            {
                case "temputer":
                    ydatas = await _graphDataService.GetGraphDataAsync(DeviceId, GraphInterval, new Dictionary<string, string>
                        {{ "模具温度", "value05" },
                        { "料筒一段温度" , "value06" },
                        { "料筒二段温度" , "value07" },
                        { "喷嘴温度" , "value08" },
                        { "设定模具温度" , "value09" }});
                    break;


                case "stress":
                    ydatas = await _graphDataService.GetGraphDataAsync(DeviceId, GraphInterval, new Dictionary<string, string>
                        {{ "注射压力", "value10" },
                        { "保压压力" , "value11" },
                        { "锁模力" , "value12" },
                        { "注射速度" , "value13" },
                        { "螺杆转速" , "value14" }});
                    break;


                case "yield":
                    ydatas = await _graphDataService.GetGraphDataAsync
                        (DeviceId, GraphInterval, new Dictionary<string, string>
                        {{ "今日产量", "value15" },
                        { "良品数量" , "value16" },
                        { "不良品数量" , "value17" },
                        { "良品率" , "value18" },
                        { "当前班次产量" , "value19" }});
                    break;


                case "status":
                    ydatas = await _graphDataService.GetGraphDataAsync
                        (DeviceId, GraphInterval, new Dictionary<string, string>
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



            List<string> xlables = GetXLables(ydatas.FirstOrDefault().Value);

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
                    ShowSeparatorLines = false,
                    Padding = new LiveChartsCore.Drawing.Padding(5,10,5,10),
                };
                index += 1;

                return Yaxe;
            }).ToArray();
        }

        public List<string> GetXLables(List<GraphDataModel> graphDataModels)
        {
            return GraphInterval switch
            {
                GraphInterval.Min => graphDataModels.Select(data => data.dateTime.ToString("HH:mm")).ToList(),
                GraphInterval.Hour => graphDataModels.Select(data => data.dateTime.ToString("HH:mm")).ToList(),
                GraphInterval.Day => graphDataModels.Select(data => data.dateTime.ToString("M.d")).ToList(),
                GraphInterval.Week => graphDataModels.Select(data => data.dateTime.ToString("M.d")).ToList(),
                GraphInterval.Month => graphDataModels.Select(data => data.dateTime.ToString("yy.M")).ToList(),
                _ => graphDataModels.Select(data => data.dateTime.ToString("HH:mm")).ToList()
            };
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

        private GraphInterval _graphInterval = GraphInterval.Min;

        public GraphInterval GraphInterval
        {
            get { return _graphInterval; }
            set { _graphInterval = value; }
        }

        private string _graphItem = "temputer";

        public string GraphItem
        {
            get { return _graphItem; }
            set { _graphItem = value; }
        }

        private string _ipAddress;
        public string IpAddress
        {
            get => _ipAddress;
            set => SetProperty(ref _ipAddress, value);
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

        

        
    }
}
