using IndustrialMonitor.Communication.IServices;
using IndustrialMonitor.Core.GraphModels;
using IndustrialMonitor.Core.Models;
using IndustrialMonitor.DataAcquisition.IServices;
using IndustrialMonitor.Modules.Dashboard.Tools;
using IndustrialMonitor.Modules.Device;
using IndustrialMonitor.Modules.Device.Tools;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Prism.Navigation.Regions;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using Windows.Services.Maps.Guidance;

namespace IndustrialMonitor.Modules.Dashboard.ViewModels
{
    public class DashboardWindowViewModel : BindableBase, IDialogAware
    {
        
        private IDeviceStorageService _deviceStorageService;
        public IGraphDataService _graphDataService;
        private readonly IDeviceCommunicationService _deviceCommunicationService;
        
        public string Title { get; } = "设备详情";

        private CancellationTokenSource? _cts;
        bool isCollecting = false;
        DetailHelper detailHelper = new();

        
        public DelegateCommand<string> ChangeValuesCommand { get; set; }

        

        

        private NavigationParameters navParams;

        public NavigationParameters NavParams
        {
            get { return navParams; }
            set { navParams = value; }
        }

        Dictionary<string, List<GraphDataModel>> ydatas;

        public DashboardWindowViewModel(
            IDeviceCommunicationService deviceCommunicationService, 
            IRegionManager regionManager, 
            IDeviceStorageService deviceStorageService,
            IGraphDataService graphDataService)
        {
            _deviceCommunicationService = deviceCommunicationService;
            _deviceStorageService = deviceStorageService;
            _graphDataService = graphDataService;

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
            StopCollectAsync();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _cts = new CancellationTokenSource();

            IpAddress = parameters.GetValue<string>("IpAddress");
            detailHelper.InitDeviceDetailItems();
           
            _ = StartCollectAsync();
            _ = getDeviceId(IpAddress);


        }

        public async Task getDeviceId(string ipAddress)
        {
            DeviceId = await _deviceStorageService.GetDeviceIdAsync(IpAddress);
        }
        #endregion

        #region CollectData
        public async Task ReadregistersAsync()
        {
            if (_cts == null) return;

            CancellationToken cancellationToken = _cts.Token;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    ReadRegistersResult readRegistersResult = await _deviceCommunicationService.ReadHoldingRegistersAsync(IpAddress);
                    if (readRegistersResult.ErrorMessage != null)
                    {
                        MessageBox.Show(readRegistersResult.ErrorMessage);
                    }

                    detailHelper.UpdateDeviceDetails(readRegistersResult.Data);

                    DeviceDetails = detailHelper.DeviceDetails;
                    TemperatureItems = detailHelper.TemperatureItems;
                    ProcessItems = detailHelper.ProcessItems;
                    ProductionItems = detailHelper.ProcessItems;
                    RunningItems = detailHelper.RunningItems;

                    DeviceStatus = DeviceDetails[0].Value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.InnerException?.Message ?? ex.Message);
                    return;
                }

                await Task.Delay(1000, cancellationToken);
            }
        }

        private async Task StartCollectAsync()
        {
            if(isCollecting) return;

            isCollecting = true;
            await ReadregistersAsync();
        }

        private void StopCollectAsync()
        {
            if (!isCollecting) return;

            _cts?.Cancel();
            _cts?.Dispose();

            isCollecting = false;
        }


        private string _deviceName;
        public string DeviceName
        {
            get => _deviceName; 
            set => SetProperty(ref _deviceName, value);
        }

        private string _ipAddress;

        public string IpAddress
        {
            get => _ipAddress;
            set => SetProperty(ref _ipAddress, value);
        }

        private string _deviceStatus;

        public string DeviceStatus
        {
            get => _deviceStatus;
            set => SetProperty(ref _deviceStatus, value);
        }

        #region ObservableCollection<DeviceDetailModel>

        public ObservableCollection<DeviceDetailModel> DeviceDetails { get; set; } = [];


        private ObservableCollection<DeviceDetailModel> _temperatureItems;

        public ObservableCollection<DeviceDetailModel> TemperatureItems
        {
            get { return _temperatureItems; }
            set => SetProperty(ref _temperatureItems, value);
        }

        private ObservableCollection<DeviceDetailModel> _processItems;

        public ObservableCollection<DeviceDetailModel> ProcessItems
        {
            get { return _processItems; }
            set => SetProperty(ref _processItems, value);
        }

        private ObservableCollection<DeviceDetailModel> _productionItems;

        public ObservableCollection<DeviceDetailModel> ProductionItems
        {
            get { return _productionItems; }
            set => SetProperty(ref _productionItems, value);
        }

        private ObservableCollection<DeviceDetailModel> _runningItems;

        public ObservableCollection<DeviceDetailModel> RunningItems
        {
            get { return _runningItems; }
            set => SetProperty(ref _runningItems, value);
        }

        #endregion

        #endregion

        #region GraphData

        private async Task ChangeValues(string parameter)
        {
            if (ydatas != null) ydatas.Clear();
            ydatas = new Dictionary<string, List<GraphDataModel>>();
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

            LoadChart(ydatas);
        }

        public void LoadChart(Dictionary<string, List<GraphDataModel>> ydatas)
        {

            Series = [];
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
                };
                index++;

                return series;
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
    }
}
