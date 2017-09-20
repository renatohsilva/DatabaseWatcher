using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableWatcher.Adapter;
using TableWatcher.Base;
using TableWatcher.Interface;

namespace TableWatcher
{
    public class TableWatcherStrategy<T> where T : class
    {
        private ITableWatcher<T> TableWatcherAbstract;

        public TableWatcherStrategy()
        {
            TableWatcherAbstract = new AdapterFactory<T>().GetAdapter();
        }

        public void InitializeTableWatcher()
        {
            TableWatcherAbstract.InitializeTableWatcher();
        }

        public void StartTableWatcher()
        {
            TableWatcherAbstract.StartTableWatcher();
        }

        public void StopTableWatcher()
        {
            TableWatcherAbstract.StopTableWatcher();
        }
    }
}
