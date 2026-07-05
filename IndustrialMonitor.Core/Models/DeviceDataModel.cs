using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Core.Models
{

    public class DeviceDataModel
    {
        public long Id { get; set; }
        public Guid DeviceId { get; set; }

        public DateTime DateTime { get; set; }


        #region register
        public ushort value00 { get; set; }
        public ushort value01 { get; set; }
        public ushort value02 { get; set; }
        public ushort value03 { get; set; }
        public ushort value04 { get; set; }
        public ushort value05 { get; set; }
        public ushort value06 { get; set; }
        public ushort value07 { get; set; }
        public ushort value08 { get; set; }
        public ushort value09 { get; set; }

        public ushort value10 { get; set; }
        public ushort value11 { get; set; }
        public ushort value12 { get; set; }
        public ushort value13 { get; set; }
        public ushort value14 { get; set; }
        public ushort value15 { get; set; }
        public ushort value16 { get; set; }
        public ushort value17 { get; set; }
        public ushort value18 { get; set; }
        public ushort value19 { get; set; }

        public ushort value20 { get; set; }
        public ushort value21 { get; set; }
        public ushort value22 { get; set; }
        public ushort value23 { get; set; }
        public ushort value24 { get; set; }
        public ushort value25 { get; set; }
        public ushort value26 { get; set; }
        public ushort value27 { get; set; }
        public ushort value28 { get; set; }
        public ushort value29 { get; set; }

        #endregion
    }
}
