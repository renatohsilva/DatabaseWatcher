using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableWatcher;
using TableWatcher.Classes;

namespace DatabaseWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            WatcherOracle();
            //WatcherSqlServer();
        }

        private static void WatcherSqlServer()
        {
            var ConnectionStringSqlServer = @"";
            var TableWatcher = new TableWatcherStrategy<Pessoa>(new TableWatcherSqlServer<Pessoa>(ConnectionStringSqlServer));
            try
            {
                TableWatcher.InitializeTableWatcher();
                TableWatcher.StartTableWatcher();
                Console.WriteLine("Esperando para receber notificações...");
                Console.WriteLine("Precione qualquer tecla para sair");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                TableWatcher.StopTableWatcher();
            }
        }

        private static void WatcherOracle()
        {
            var ConnectionStringOracle = @"";
            var TableWatcher = new TableWatcherStrategy<Pessoa>(new TableWatcherOracle<Pessoa>(ConnectionStringOracle));

            try
            {
                TableWatcher.InitializeTableWatcher();
                TableWatcher.StartTableWatcher();
                Console.WriteLine("Esperando para receber notificações...");
                Console.WriteLine("Precione qualquer tecla para sair");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                TableWatcher.StopTableWatcher();
            }
        }
    }
}
