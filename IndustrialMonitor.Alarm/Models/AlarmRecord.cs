using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Alarm.Models
{
    public class AlarmRecord
    {
        [Key]
        public int id { get; set; }

        public Guid DeviceId { get; set; }

        public enum Level
        {
            Normal,
            Warning,
            Critical
        }

        public Level AlarmLevel { get; set; } = Level.Normal;

        public int AlarmCode { get; set; }


        public enum Item
        {
            Exception,
            DbUpdateException_SqliteException,
            IOException_SqliteException,
            SocketException,
            TimeoutException,
            SlaveException,
            SqliteException,
            DbUpdateException,
            IOException,
            ArgumentException,
            InvalidOperationException,
            UnknowException
        }

        public Item AlarmItem { get; set; } = Item.Exception;

        public string Message { get; set; } = string.Empty;

        public DateTime DateTime { get; set; } = DateTime.Now;

        public bool IsRecovered { get; set; } = false;
        
        public DateTime? RecoveredAt { get; set; }

    }
}
