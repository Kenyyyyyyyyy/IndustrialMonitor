using IndustrialMonitor.Core.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Core.IRepository
{
    public interface IBaseRepository<T> where T : class
    {
        public Task RecordData(T entity);

        public Task DelateByDevice(T entity);
    }
}
