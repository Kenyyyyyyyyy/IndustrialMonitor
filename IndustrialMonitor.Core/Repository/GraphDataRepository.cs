using IndustrialMonitor.Core.GraphModels;
using IndustrialMonitor.Core.IRepository;
using IndustrialMonitor.Core.SqlServer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IndustrialMonitor.Core.Repository
{
    public class GraphDataRepository : IGraphDataRepository
    {
        public async Task<List<GraphDataModel>> GetDataByDeviceIdAndInterval(GraphRequestModel graphRequestModel)
        {
            await using var context = new IndustrialMonitorDbContext();
            var register = graphRequestModel.RegisterAddress;
            var interval = graphRequestModel.Interval;

            if (register == null) throw new Exception("RegisterAddress or Interval is null");

            DateTime endTime = DateTime.Now;
            DateTime startTime = GetStartTime(endTime, interval);


            var extradata = await context.DeviceDataModels
                .Where(x => x.DeviceId == graphRequestModel.DeviceId)
                .Where(x => x.DateTime >= startTime && x.DateTime <= endTime)
                .Select(x => new
                {
                    x.DateTime,
                    Value =  EF.Property<double>(x, register)
                })
                .ToListAsync();

            var data = extradata
                .GroupBy(x => GetTimeBucket(x.DateTime, interval))
                .Select(g => new GraphDataModel
                {
                    dateTime = g.Key,
                    value = Math.Round(g.Average(x => x.Value),0)
                })
                .OrderBy(x => x.dateTime)
                .ToList();

            return data;
        }

        private static DateTime GetStartTime(DateTime endTime, GraphInterval interval)
        {
            return interval switch
            {
                GraphInterval.Min => endTime.AddMinutes(-30),   
                GraphInterval.Hour => endTime.AddHours(-12),    
                GraphInterval.Day => endTime.AddDays(-10),      
                GraphInterval.Week => endTime.AddDays(-84),     
                GraphInterval.Month => endTime.AddMonths(-12),  
                _ => endTime.AddHours(-12)
            };
        }

        private static DateTime GetTimeBucket(DateTime time, GraphInterval interval)
        {
            return interval switch
            {
                // 例如 10:35:42、10:35:58 都归到 10:35:00
                GraphInterval.Min => new DateTime(
                    time.Year, time.Month, time.Day,
                    time.Hour, time.Minute, 0),

                // 同一小时归为一点
                GraphInterval.Hour => new DateTime(
                    time.Year, time.Month, time.Day,
                    time.Hour, 0, 0),

                // 同一天归为一点
                GraphInterval.Day => time.Date,

                // 每周一作为一周的起点
                GraphInterval.Week => time.Date.AddDays(
                    -((int)time.DayOfWeek + 6) % 7),

                // 每月 1 日作为一个月的起点
                GraphInterval.Month => new DateTime(
                    time.Year, time.Month, 1),

                _ => time
            };
        }

    }
}
