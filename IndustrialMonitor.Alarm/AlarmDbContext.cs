using IndustrialMonitor.Alarm.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Alarm
{
    public class AlarmDbContext : DbContext
    {
        public DbSet<AlarmRecord> AlarmRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dataDirectory = Path.Combine(AppContext.BaseDirectory,"Data");

            Directory.CreateDirectory(dataDirectory);

            string databasePath = Path.Combine(dataDirectory, "AlarmRecords.db");

            optionsBuilder.UseSqlite($"Data Source={databasePath}");
        }
    }
}


/*dotnet ef migrations add InitialCreate 
 * --project .\IndustrialMonitor.Alarm\IndustrialMonitor.Alarm.csproj 
 * --startup-project .\IndustrialMonitor\IndustrialMonitor.csproj 
 * --context AlarmDbContext*/