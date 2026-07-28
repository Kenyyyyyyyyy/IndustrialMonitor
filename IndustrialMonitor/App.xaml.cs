using IndustrialMonitor.Alarm.IRepository;
using IndustrialMonitor.Alarm.Repository;
using IndustrialMonitor.Modules.Dashboard;
using IndustrialMonitor.Modules.Device;
using IndustrialMonitor.Modules.Monitor;
using System.Configuration;
using System.Data;
using System.Windows;
using Monitor = IndustrialMonitor.Modules.Monitor.Monitor;

namespace IndustrialMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainWindow,MainWindowViewModel>();

        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<Dashboard>();
            moduleCatalog.AddModule<Device>();
            moduleCatalog.AddModule<Monitor>();
            base.ConfigureModuleCatalog(moduleCatalog);
        }

    }

}
