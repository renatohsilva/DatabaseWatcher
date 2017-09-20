using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableWatcher.Interface;
using System.Configuration;

namespace TableWatcher.Adapter
{
    public class AdapterFactory<T> where T : class
    {
        private const string OracleConnectionString = "oracleConnectionString";
        private const string SqlServerConnectionString = "sqlServerConnectionString";

        public ITableWatcher<T> GetAdapter()
        {
            var connectionString = getConnectionString(OracleConnectionString);
            if (!string.IsNullOrEmpty(connectionString))
            {
                return new TableWatcherOracle<T>(connectionString);
            }
            else
            {
                connectionString = getConnectionString(SqlServerConnectionString);
                return new TableWatcherSqlServer<T>(connectionString);
            }
        }

        private string getConnectionString(string tag)
        {
            var connectionStringSettings = ConfigurationManager.AppSettings[tag];
            return connectionStringSettings;
        }
    }
}
