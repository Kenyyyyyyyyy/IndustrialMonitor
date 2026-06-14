using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor
{
    internal class MainWindowViewModel:BindableBase
    {
        private readonly IRegionManager _RegionManager;

        public DelegateCommand<string> NavigateCommand { get; }



        public MainWindowViewModel(IRegionManager regionManager)
        {
            _RegionManager = regionManager;

            NavigateCommand = new DelegateCommand<string>((region) =>
            {
                regionManager.Regions["ContentRegion"].RequestNavigate(region);
            });
        }

    }
}
