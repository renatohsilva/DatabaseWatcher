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
            Watcher();
        }

        private static void Watcher()
        {
            var TableWatcher = new TableWatcherStrategy<Pessoa>();
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
