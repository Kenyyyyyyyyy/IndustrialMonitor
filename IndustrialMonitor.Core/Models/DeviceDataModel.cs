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

        public void SetValues(IReadOnlyList<ushort> values)
        {
            if (values.Count < 30)
                throw new ArgumentException("寄存器数量不足 30 个");

            value00 = values[0];
            value01 = values[1];
            value02 = values[2];
            value03 = values[3];
            value04 = values[4];
            value05 = values[5];
            value06 = values[6];
            value07 = values[7];
            value08 = values[8];
            value09 = values[9];

            value10 = values[10];
            value11 = values[11];
            value12 = values[12];
            value13 = values[13];
            value14 = values[14];
            value15 = values[15];
            value16 = values[16];
            value17 = values[17];
            value18 = values[18];
            value19 = values[19];

            value20 = values[20];
            value21 = values[21];
            value22 = values[22];
            value23 = values[23];
            value24 = values[24];
            value25 = values[25];
            value26 = values[26];
            value27 = values[27];
            value28 = values[28];
            value29 = values[29];
        }
    }
}
