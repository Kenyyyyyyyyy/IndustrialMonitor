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
            DateTime startTime = endTime.AddHours(-24);

            //var Data = await context.DeviceDataModels
            //.Where(x => x.DeviceId == graphRequestModel.DeviceId)
            //.Where(x => x.DateTime >= startTime && x.DateTime <= endTime)
            //.GroupBy(datetime => new { datetime.DateTime.Year, datetime.DateTime.Month, datetime.DateTime.Day, datetime.DateTime.Hour })
            //.Select(g => new GraphDataModel
            //{
            //    dateTime = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day, g.Key.Hour, 0, 0),
            //    value = (ushort)g.Average(x => EF.Property<ushort>(x, register))
            //})
            //.OrderBy(x => x.dateTime)
            //.ToListAsync();

            //return Data;

            var extradata = await context.DeviceDataModels
                .Where(x => x.DeviceId == graphRequestModel.DeviceId)
                .Where(x => x.DateTime >= startTime && x.DateTime <= endTime)
                .Select(x => new
                {
                    x.DateTime,
                    Value =  EF.Property<double>(x, register)
                }).ToListAsync();

            var data = extradata
                .GroupBy(x => new { x.DateTime.Year, x.DateTime.Month, x.DateTime.Day, x.DateTime.Hour })
                .Select(g => new GraphDataModel
                {
                    dateTime = new DateTime
                    (
                        g.Key.Year,
                        g.Key.Month,
                        g.Key.Day,
                        g.Key.Hour,
                        0,
                        0),
                    value = g.Average(x => x.Value)
                }).OrderBy(x => x.dateTime).ToList();

            return data;
        }
    }
}
