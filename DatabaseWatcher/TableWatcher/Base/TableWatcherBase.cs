using System;
using System.Collections.Generic;
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
        protected String nomeEntidadeSqlServer = typeof(T).Name;
        protected String nomeEntidadeOracle = typeof(T).Name.ToUpper();
        protected IList<String> listaUpdate;

        protected virtual void MapearEntidade()
        {
            mapper = new ModelToTableMapper<T>();
            listaUpdate = new List<String>();
            foreach (var prop in GetValues())
            {
                mapper.AddMapping(prop.Key, prop.Value);
                listaUpdate.Add(prop.Value);
            }
        }

        private Dictionary<PropertyInfo, string> GetValues()
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
    }
}
