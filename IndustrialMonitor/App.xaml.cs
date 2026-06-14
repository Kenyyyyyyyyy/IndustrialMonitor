using IndustrialMonitor.Modules.Dashboard;
using System.Configuration;
using System.Data;
using System.Windows;

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
            base.ConfigureModuleCatalog(moduleCatalog);
        }

    }

}
