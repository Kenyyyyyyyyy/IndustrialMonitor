using IndustrialMonitor.Alarm.IRepository;
using IndustrialMonitor.Alarm.Models;
using IndustrialMonitor.Core.IRepository;
using IndustrialMonitor.DataAcquisition.IServices;
using IndustrialMonitor.EventSupport.Events;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NModbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.DataAcquisition.Services
{
    public class AlarmRecordService : IAlarmRecordService
    {

        private readonly Alarm.IRepository.IBaseRepository<AlarmRecord> _baseRepository;
        private readonly IEventAggregator _eventAggregator;

        public AlarmRecordService(Alarm.IRepository.IBaseRepository<AlarmRecord> baseRepository, IEventAggregator eventAggregator) 
        {
            _baseRepository = baseRepository;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<CommunicationFailedEvent>()
                            .Subscribe(RecordAlarmAsync);
        }

        public void RecordAlarmAsync(ErrorInfo errorInfo)
        {
            AlarmRecord record = new AlarmRecord()
            {
                DeviceId = errorInfo.deviceid,
                Message = errorInfo.exception.Message,
                DateTime = DateTime.Now,
            };

            Exception exception = errorInfo.exception;

            if (exception is DbUpdateException dbUpdateException &&
                dbUpdateException.InnerException is SqliteException sqliteException)
            {
                record.AlarmCode = 5;
                record.AlarmLevel = AlarmRecord.Level.Critical;
                record.Message = sqliteException.Message;
                record.AlarmItem = AlarmRecord.Item.DbUpdateException_SqliteException;
            }
            else if (exception is IOException ioException &&
                     ioException.InnerException is SocketException socketException)
            {
                record.AlarmCode = 1;
                record.AlarmLevel = AlarmRecord.Level.Critical;
                record.Message = socketException.Message;
                record.AlarmItem = AlarmRecord.Item.IOException_SqliteException;
            }
            else
            {
                switch (exception)
                {
                    case SocketException:
                        record.AlarmCode = 1;
                        record.AlarmLevel = AlarmRecord.Level.Critical;
                        record.AlarmItem = AlarmRecord.Item.SocketException;
                        break;

                    case TimeoutException:
                        record.AlarmCode = 2;
                        record.AlarmLevel = AlarmRecord.Level.Critical;
                        record.AlarmItem = AlarmRecord.Item.TimeoutException;
                        break;

                    case SlaveException:
                        record.AlarmCode = 3;
                        record.AlarmLevel = AlarmRecord.Level.Warning;
                        record.AlarmItem = AlarmRecord.Item.SlaveException;
                        break;

                    case SqliteException:
                        record.AlarmCode = 5;
                        record.AlarmLevel = AlarmRecord.Level.Critical;
                        record.AlarmItem = AlarmRecord.Item.SqliteException;
                        break;

                    case DbUpdateException:
                        record.AlarmCode = 4;
                        record.AlarmLevel = AlarmRecord.Level.Critical;
                        record.AlarmItem = AlarmRecord.Item.DbUpdateException;
                        break;

                    case IOException:
                        record.AlarmCode = 0;
                        record.AlarmLevel = AlarmRecord.Level.Critical;
                        record.AlarmItem = AlarmRecord.Item. IOException;
                        break;

                    case ArgumentException:
                        record.AlarmCode = 6;
                        record.AlarmLevel = AlarmRecord.Level.Warning;
                        record.AlarmItem = AlarmRecord.Item. ArgumentException;
                        break;

                    case InvalidOperationException:
                        record.AlarmCode = 7;
                        record.AlarmLevel = AlarmRecord.Level.Warning;
                        record.AlarmItem = AlarmRecord.Item. InvalidOperationException;
                        break;

                    default:
                        record.AlarmCode = 999;
                        record.AlarmLevel = AlarmRecord.Level.Warning;
                        record.AlarmItem = AlarmRecord.Item.UnknowException;
                        break;
                }
            }

            _baseRepository.RecordAlarm(record);
        }
    }
}
