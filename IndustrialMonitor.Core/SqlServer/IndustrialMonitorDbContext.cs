using IndustrialMonitor.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Core.SqlServer
{
    public class IndustrialMonitorDbContext : DbContext
    {
        public DbSet<DeviceDataModel> DeviceDataModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            base.OnConfiguring(dbContextOptionsBuilder);
            dbContextOptionsBuilder.UseSqlServer("Server=SAISYORIN\\SQLEXPRESS;uid=sa;pwd=mmsdsl123;Database=IndustrialMonitor;Trusted_Connection=True;TrustServerCertificate=True;");
            //dbContextOptionsBuilder.UseSqlServer("Server=127.0.0.1;uid=sa;pwd=mmsdsl123;Database=IndustrialMonitor;Trusted_Connection=True;TrustServerCertificate=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DeviceDataModel>()
            .Property(x => x.DateTime)
            .HasPrecision(0);
        }

    }
}
