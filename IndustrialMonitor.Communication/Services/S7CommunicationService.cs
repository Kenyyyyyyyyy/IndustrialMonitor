using IndustrialMonitor.Communication.IServices;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Communication.Services
{
    public class S7CommunicationService : IS7CommunicationService
    {
        public void S7Connect()
        {
            Plc plc = new Plc(CpuType.S71500, "192.168.126.128", 0, 2);

            plc.Open();

            plc.GetHashCode();
        }

        
    }
}
