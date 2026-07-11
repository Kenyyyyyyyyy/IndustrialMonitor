using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Communication.IServices
{
    public interface IModbusSimulationService
    {
        public Task StartSimulation(string ipAddress);
    }
}
