using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TableDependency;
using TableDependency.Mappers;

namespace TableWatcher.Base
{
    public abstract class TableWatcherBase<T> where T : class
    {
        protected ModelToTableMapper<T> mapper;

        protected String nomeEntidadeSqlServer = GetNomeEntidadeSqlServer(17); //Nas versões atuais do banco de dados caso o nome possua mais de 17 caracteres será retornado um erro;
        protected String nomeEntidadeOracle = GetNomeEntidadeSqlServer(17).ToUpper(); //Nas versões atuais do banco de dados caso o nome possua mais de 17 caracteres será retornado um erro;

        protected IList<String> listaUpdate;

        protected virtual void MapearEntidade()
        {
            mapper = new ModelToTableMapper<T>();
            listaUpdate = new List<String>();
            foreach (var prop in GetValuesForMapper())
            {
                mapper.AddMapping(prop.Key, prop.Value);
                listaUpdate.Add(prop.Value);
            }
        }

        private Dictionary<PropertyInfo, string> GetValuesForMapper()
        {
            Dictionary<PropertyInfo, string> valores = new Dictionary<PropertyInfo, string>();

            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    var authAttr = attr as AtributoBanco;
                    if (authAttr != null)
                    {
                        var auth = authAttr.NomeBanco;
                        valores.Add(prop, auth);
                    }
                }
            }

            return valores;
        }

        private static string GetNomeEntidadeSqlServer(int TamanhoMaximo)
        {
            return typeof(T).Name.Length > TamanhoMaximo ? typeof(T).Name.Substring(0, TamanhoMaximo) : typeof(T).Name;
        }

        protected virtual SqlCommand MontaInsertCommand(SqlConnection connection, TableDependency.EventArgs.RecordChangedEventArgs<T> e)
        {
            SqlCommand command = new SqlCommand();

            Dictionary<String, object> dicionarioValoresInsert = GetValuesForInsert(e);

            string fields = String.Join(",", dicionarioValoresInsert.Select(s => s.Key));
            string values = String.Join(",", dicionarioValoresInsert.Select(s => $"@{s.Key}"));

            string sql = $"INSERT INTO {nomeEntidadeSqlServer}TYPE ({fields}) values ({values})";
            foreach (var item in dicionarioValoresInsert)
            {
                command.Parameters.AddWithValue(item.Key, item.Value);
            }

            command.Connection = connection;
            command.CommandText = sql;

            return command;
        }

        protected virtual OracleCommand MontaInsertCommand(OracleConnection connection, TableDependency.EventArgs.RecordChangedEventArgs<T> e)
        {
            OracleCommand command = new OracleCommand();

            Dictionary<String, object> dicionarioValoresInsert = GetValuesForInsert(e);

            string fields = String.Join(",", dicionarioValoresInsert.Select(s => s.Key));
            string values = String.Join(",", dicionarioValoresInsert.Select(s => $"@{s.Key}"));

            string sql = $"INSERT INTO {nomeEntidadeOracle}TYPE ({fields}) values ({values})";
            foreach (var item in dicionarioValoresInsert)
            {
                command.Parameters.Add(item.Key, item.Value);
            }

            command.Connection = connection;
            command.CommandText = sql;

            return command;
        }


        private Dictionary<String, object> GetValuesForInsert(TableDependency.EventArgs.RecordChangedEventArgs<T> evento)
        {
            Dictionary<String, object> valores = new Dictionary<String, object>();

            List<PropertyInfo> propriedades = new List<PropertyInfo>(evento.Entity.GetType().GetProperties());

            foreach (PropertyInfo prop in propriedades)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    var authAttr = attr as AtributoBanco;
                    object valorPropriedade = prop.GetValue(evento.Entity, null);
                    valores.Add(authAttr.NomeBanco, valorPropriedade);
                }
            }

            int valorOperacao = Convert.ToInt32(evento.ChangeType);
            valores.Add("TipoOperacao", valorOperacao.ToString());

            return valores;
        }
    }
}
