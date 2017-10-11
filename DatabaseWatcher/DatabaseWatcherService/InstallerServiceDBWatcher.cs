using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace BennerESocialDbWatcherService
{
    [RunInstaller(true)]
    public partial class InstallerServiceDBWatcher : System.Configuration.Install.Installer
    {
        public InstallerServiceDBWatcher()
        {
            InitializeComponent();
        }
    }
}
