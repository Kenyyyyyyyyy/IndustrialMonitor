using IndustrialMonitor.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Modules.Dashboard
{
    internal class DetailHelper
    {

        public ObservableCollection<DeviceDetailModel> DeviceDetails { get; set; } = [];
        public ObservableCollection<DeviceDetailModel> TemperatureItems { get; set; } = [];
        public ObservableCollection<DeviceDetailModel> ProcessItems { get; set; } = [];
        public ObservableCollection<DeviceDetailModel> ProductionItems { get; set; } = [];
        public ObservableCollection<DeviceDetailModel> RunningItems { get; set; } = [];

        public void InitDeviceDetailItems()
        {
            DeviceDetails.Clear();
            TemperatureItems.Clear();
            ProcessItems.Clear();
            ProductionItems.Clear();
            RunningItems.Clear();

            DeviceDetails.Add(new DeviceDetailModel { Name = "设备状态", Value = "未知", Unit = "" });
            DeviceDetails.Add(new DeviceDetailModel { Name = "自动/手动模式", Value = "未知", Unit = "" });
            DeviceDetails.Add(new DeviceDetailModel { Name = "通信心跳", Value = "0", Unit = "" });
            DeviceDetails.Add(new DeviceDetailModel { Name = "当前报警状态", Value = "正常", Unit = "" });
            DeviceDetails.Add(new DeviceDetailModel { Name = "当前报警代码", Value = "0", Unit = "" });

            TemperatureItems.Add(new DeviceDetailModel { Name = "模具温度", Value = "0", Unit = "℃" });
            TemperatureItems.Add(new DeviceDetailModel { Name = "料筒一段温度", Value = "0", Unit = "℃" });
            TemperatureItems.Add(new DeviceDetailModel { Name = "料筒二段温度", Value = "0", Unit = "℃" });
            TemperatureItems.Add(new DeviceDetailModel { Name = "喷嘴温度", Value = "0", Unit = "℃" });
            TemperatureItems.Add(new DeviceDetailModel { Name = "设定模具温度", Value = "0", Unit = "℃" });

            ProcessItems.Add(new DeviceDetailModel { Name = "注射压力", Value = "0", Unit = "MPa" });
            ProcessItems.Add(new DeviceDetailModel { Name = "保压压力", Value = "0", Unit = "MPa" });
            ProcessItems.Add(new DeviceDetailModel { Name = "锁模力", Value = "0", Unit = "kN" });
            ProcessItems.Add(new DeviceDetailModel { Name = "注射速度", Value = "0", Unit = "mm/s" });
            ProcessItems.Add(new DeviceDetailModel { Name = "螺杆转速", Value = "0", Unit = "rpm" });

            ProductionItems.Add(new DeviceDetailModel { Name = "今日产量", Value = "0", Unit = "件" });
            ProductionItems.Add(new DeviceDetailModel { Name = "良品数量", Value = "0", Unit = "件" });
            ProductionItems.Add(new DeviceDetailModel { Name = "不良品数量", Value = "0", Unit = "件" });
            ProductionItems.Add(new DeviceDetailModel { Name = "良品率", Value = "0", Unit = "%" });
            ProductionItems.Add(new DeviceDetailModel { Name = "当前班次产量", Value = "0", Unit = "件" });

            RunningItems.Add(new DeviceDetailModel { Name = "当前循环周期", Value = "0", Unit = "秒" });
            RunningItems.Add(new DeviceDetailModel { Name = "标准循环周期", Value = "0", Unit = "秒" });
            RunningItems.Add(new DeviceDetailModel { Name = "运行时间", Value = "0", Unit = "分钟" });
            RunningItems.Add(new DeviceDetailModel { Name = "停机时间", Value = "0", Unit = "分钟" });
            RunningItems.Add(new DeviceDetailModel { Name = "稼动率", Value = "0", Unit = "%" });
        }

        public void UpdateDeviceDetails(ushort[] registers)
        {
            DeviceDetails[0].Value = GetDeviceStatusText(registers[0]);
            DeviceDetails[1].Value = GetWorkModeText(registers[1]);
            DeviceDetails[2].Value = registers[2].ToString();
            DeviceDetails[3].Value = GetAlarmStatusText(registers[3]);
            DeviceDetails[4].Value = GetAlarmCodeText(registers[4]);

            // 温度类：寄存器值 / 10.0
            TemperatureItems[0].Value = (registers[5] / 10.0).ToString("F1");
            TemperatureItems[1].Value = (registers[6] / 10.0).ToString("F1");
            TemperatureItems[2].Value = (registers[7] / 10.0).ToString("F1");
            TemperatureItems[3].Value = (registers[8] / 10.0).ToString("F1");
            TemperatureItems[4].Value = (registers[9] / 10.0).ToString("F1");

            // 工艺参数
            ProcessItems[0].Value = (registers[10] / 10.0).ToString("F1");
            ProcessItems[1].Value = (registers[11] / 10.0).ToString("F1");
            ProcessItems[2].Value = (registers[12] / 10.0).ToString("F1");
            ProcessItems[3].Value = (registers[13] / 10.0).ToString("F1");
            ProcessItems[4].Value = registers[14].ToString();

            // 生产统计
            ProductionItems[0].Value = registers[15].ToString();
            ProductionItems[1].Value = registers[16].ToString();
            ProductionItems[2].Value = registers[17].ToString();
            ProductionItems[3].Value = (registers[18] / 10.0).ToString("F1");
            ProductionItems[4].Value = registers[19].ToString();

            // 稼动分析
            RunningItems[0].Value = (registers[20] / 10.0).ToString("F1");
            RunningItems[1].Value = (registers[21] / 10.0).ToString("F1");
            RunningItems[2].Value = registers[22].ToString();
            RunningItems[3].Value = registers[23].ToString();
            RunningItems[4].Value = (registers[24] / 10.0).ToString("F1");
        }


        private string GetDeviceStatusText(ushort value)
        {
            return value switch
            {
                0 => "停机",
                1 => "运行",
                2 => "待机",
                3 => "故障",
                _ => "未知"
            };
        }

        private string GetWorkModeText(ushort value)
        {
            return value switch
            {
                0 => "手动",
                1 => "自动",
                _ => "未知"
            };
        }

        private string GetAlarmStatusText(ushort value)
        {
            return value switch
            {
                0 => "正常",
                1 => "报警",
                _ => "未知"
            };
        }

        private string GetAlarmCodeText(ushort value)
        {
            return value switch
            {
                0 => "无报警",
                101 => "模具温度过高",
                102 => "料筒温度过高",
                103 => "喷嘴温度过高",
                201 => "注射压力异常",
                202 => "保压压力异常",
                301 => "循环周期超时",
                401 => "通信异常",
                501 => "急停触发",
                601 => "良品率过低",
                _ => $"未知报警({value})"
            };
        }
    }
}
