using IndustrialMonitor.Alarm.Models;
using IndustrialMonitor.DataAcquisition.IServices;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Modules.Monitor.ViewModels
{
    public class MonitorViewModel : BindableBase, INavigationAware
    {

        private readonly IAlarmDataService _alarmDataService;

        public MonitorViewModel(IAlarmDataService alarmDataService)
        {
            _alarmDataService = alarmDataService;

            ChangePageCommand = new(para =>
            {
                if (para == "up" && PageIndex > 0)
                {
                    PageIndex --;
                    _ = GetRecordListAsync();
                }
                if (para == "down")
                {
                    PageIndex ++;
                    _ = GetRecordListAsync();
                }
            });
        }

        

        public DelegateCommand<string> ChangePageCommand { get; set; }

        public async Task GetRecordListAsync()
        {
            AlarmRecordList.Clear();
            List<AlarmRecord> records = await _alarmDataService.GetAlarmRecords(PageIndex,pageSize);

            foreach (var record in records)
            {
                AlarmRecordList.Add(record);
            }
        }

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _ = GetRecordListAsync();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        #endregion

        private ObservableCollection<AlarmRecord> _alarmRecordList = [];

        public ObservableCollection<AlarmRecord> AlarmRecordList
        {
            get => _alarmRecordList;
            set => SetProperty(ref _alarmRecordList, value);
        }

        private byte _pageIndex = 1;

        public byte PageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value; }
        }

        private byte _pageSize = 10;

        public byte pageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }
    }


}
