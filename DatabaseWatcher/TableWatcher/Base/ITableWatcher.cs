using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableWatcher.Base
{
    public interface ITableWatcher<T> where T : class
    {
        void InitializeTableWatcher();
        void StartTableWatcher();
        void StopTableWatcher();
        void OnError(object sender, TableDependency.EventArgs.ErrorEventArgs e);
        void OnChanged(object sender, TableDependency.EventArgs.RecordChangedEventArgs<T> e);
    }
}
