using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.DataAcquisition.IServices
{
    public interface IAcquisitionService
    {

        public Task StartCollectAsync(string ipAddress);

        public void StopCollectAsync(string ipAddress);
    }
}
