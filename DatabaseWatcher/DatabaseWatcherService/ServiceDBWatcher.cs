using BennerESocialDbWatcherService.BLL;
using BennerESocialDbWatcherService.Model;
using System.ServiceProcess;
using TableWatcher;
using TableWatcher.Factory;

namespace BennerESocialDbWatcherService
{
    public partial class ServiceDBWatcher : ServiceBase
    {
        TableWatcherInitializer TableWatcher;

        public ServiceDBWatcher()
        {
            InitializeComponent();
            TableWatcher = new TableWatcherInitializer();
        }

        protected override void OnStart(string[] args)
        {
            TableWatcher.InitializeTableWatchers();
            TableWatcher.StartTableWatchers();
        }

        protected override void OnStop()
        {
            TableWatcher.StopTableWatchers();
        }
    }
}
