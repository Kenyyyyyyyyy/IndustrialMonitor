using IndustrialMonitor.Core.GraphModels;
using IndustrialMonitor.Core.IRepository;
using IndustrialMonitor.Core.Models;
using IndustrialMonitor.Core.SqlServer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Core.Repository
{
    public class BaseReposity : IBaseRepository<DeviceDataModel>
    {
        public async Task RecordData(DeviceDataModel entity)
        {
            await using var context = new IndustrialMonitorDbContext();
            context.Add(entity);
            await context.SaveChangesAsync();
        }

        public async Task DelateByDevice(DeviceDataModel entity)
        {
            await using var context = new IndustrialMonitorDbContext();

            var DeviceList = context.DeviceDataModels.Where(x => x.DeviceId == entity.DeviceId).ToList();
            foreach (var Device in DeviceList)
            {
                if (Device != null)
                {
                    context.DeviceDataModels.Remove(Device);
                }
            }
        }

        
    }
}
