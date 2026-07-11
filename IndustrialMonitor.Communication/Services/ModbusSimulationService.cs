using IndustrialMonitor.Communication.IServices;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Communication.Services
{
    public class ModbusSimulationService : IModbusSimulationService
    {

        private readonly IDeviceCommunicationService _deviceCommunicationService;

        ObservableCollection<string>? ipAddressList;

        public ModbusSimulationService(IDeviceCommunicationService deviceCommunicationService)
        {
            _deviceCommunicationService = deviceCommunicationService;
        }

        public async Task StartSimulation(string ipAddress)
        {
            if (ipAddress != null)
            {
                var result = await _deviceCommunicationService.ReadHoldingRegistersAsync(ipAddress);
                ushort[] resultData = result.Data;
                ushort[] registers = new ushort[20];

                int i = 0;
                for (ushort index = 5; index <= 24; index++)
                {
                    ushort newValue = RandomRegisterValue(resultData[index]);
                    registers[i] = newValue;
                    i++;
                }
                await _deviceCommunicationService.WriteRegisterAsync(ipAddress, 5, registers);
            }
        }
    

        

        public ushort RandomRegisterValue(ushort value, ushort min = 0,ushort max = 2000)
        {
            var change = Random.Shared.Next(-20, 20);
            var next = value + change;
            next = Math.Clamp(value + change, min, max);

            return (ushort)next;
            
        }



    }
}
