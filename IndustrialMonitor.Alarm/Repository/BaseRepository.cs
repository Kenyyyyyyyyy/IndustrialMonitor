using IndustrialMonitor.Alarm.IRepository;
using IndustrialMonitor.Alarm.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Alarm.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {

        public async Task RecordAlarm(T entity)
        {
            await using var context = new AlarmDbContext();
            await context.Set<T>().AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task<List<AlarmRecord>> GetAlarmRecordsAsync(byte pageIndex, byte pageSize)
        {
            await using var context = new AlarmDbContext();
            var records = context.AlarmRecords
                .OrderByDescending(x => x.DateTime)
                .Skip(pageSize*(pageIndex - 1))
                .Take(pageSize)
                .ToList();

            return records;
        }

    }
}
