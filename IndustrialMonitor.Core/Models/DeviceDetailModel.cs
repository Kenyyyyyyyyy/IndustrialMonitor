using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Core.Models
{
    public class DeviceDetailModel : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public string Unit { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
