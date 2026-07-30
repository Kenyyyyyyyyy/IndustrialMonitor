using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IndustrialMonitor.Core.Models.DeviceConfigModels
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(ModbusTcpDeviceConfig), "modbusTcp")]
    [JsonDerivedType(typeof(ModbusRtuDeviceConfig), "modbusRtu")]
    [JsonDerivedType(typeof(SiemensS7DeviceConfig), "siemensS7")]

    public class DeviceConfig
    {
        public Guid DeviceId { get; set; } = Guid.NewGuid();

        public string DeviceName { get; set; } = string.Empty;

        public DeviceType DeviceType { get; set; }

    }

    public enum DeviceType
    {
        ModbusTcp,
        ModbusRtu,
        SiemensS7
    }
}
