using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Modules.Device.Tools
{
    public interface IDeviceStorageService
    {
        public Task<Guid> GetDeviceIdAsync(string ipAddress);
    }
}
