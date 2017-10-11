using System.ComponentModel;
using System.ServiceProcess;

namespace BennerESocialDbWatcherService
{
    [RunInstaller(true)]
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ServiceDBWatcher()
            };

            ServiceBase.Run(ServicesToRun);
        }
    }
}
