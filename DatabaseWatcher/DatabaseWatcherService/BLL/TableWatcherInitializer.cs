using TableWatcher.Factory;
using BennerESocialDbWatcherService.Model;

namespace BennerESocialDbWatcherService.BLL
{
    public class TableWatcherInitializer
    {
        TableWatcherStrategy<Paises> twPaises;
        

        public TableWatcherInitializer()
        {
            twPaises = new TableWatcherStrategy<Paises>();
        }

        public void InitializeTableWatchers()
        {
            twPaises.InitializeTableWatcher();
        }

        public void StartTableWatchers()
        {
            twPaises.StartTableWatcher();
        }

        public void StopTableWatchers()
        {
            twPaises.StopTableWatcher();
        }
    }
}
