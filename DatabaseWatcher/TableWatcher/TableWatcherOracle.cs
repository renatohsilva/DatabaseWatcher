using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TableDependency.Enums;
using TableDependency.OracleClient;
using TableDependency.SqlClient;
using TableWatcher.Base;
using Oracle.ManagedDataAccess.Client;
using TableWatcher.Interface;

namespace TableWatcher
{
    public class TableWatcherOracle<T> : TableWatcherBase<T>, ITableWatcher<T> where T : class
    {
        public readonly String ConnectionString;
        private OracleTableDependency<T> _dependency;

        public TableWatcherOracle(String connectionString)
        {
            ConnectionString = connectionString;
            MapearEntidade();
        }

        public void InitializeTableWatcher()
        {
            _dependency = new OracleTableDependency<T>(ConnectionString, nomeEntidadeOracle, mapper, listaUpdate, DmlTriggerType.All, true, nomeEntidadeOracle + "TYPE");
            _dependency.OnChanged += OnChanged;
            _dependency.OnError += OnError;
        }

        public void StartTableWatcher()
        {
            _dependency.Start();
        }

        public void StopTableWatcher()
        {
            _dependency.Stop();
        }

        public void OnError(object sender, TableDependency.EventArgs.ErrorEventArgs e)
        {
            throw e.Error;
        }

        public void OnChanged(object sender, TableDependency.EventArgs.RecordChangedEventArgs<T> e)
        {
            if (e.ChangeType != ChangeType.None)
            {
                Task.Run(() => InserirOnChange(e));
            }
        }

        public async Task InserirOnChange(TableDependency.EventArgs.RecordChangedEventArgs<T> e)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await Task.Run(() => connection.OpenAsync());
                OracleCommand insertCommand = MontaInsertCommand(connection, e);
                await Task.Run(() => insertCommand.ExecuteNonQueryAsync());
            }
        }
    }
}
