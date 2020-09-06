using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;

namespace EvictPowerSrv
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            serviceInstaller1.Description = Properties.strings.Service_Description;

            // Start service after installing
            serviceInstaller1.AfterInstall += (sender, args) => new ServiceController(serviceInstaller1.ServiceName).Start();
        }
    }
}
