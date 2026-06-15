using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Modules.Dashboard
{
    public class DashboardViewModel : BindableBase
    {
        public DashboardViewModel()
        {
            TodayOutput = 1250;
        }

        private int _todayoutput;

        public int TodayOutput
        {
            get { return _todayoutput; }
            set { SetProperty(ref _todayoutput,value); }
        }

    }
}
