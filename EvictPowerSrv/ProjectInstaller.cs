using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
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

            // Eventlog Installer
            EventLogInstaller eventLogInstaller1 = new EventLogInstaller();
            eventLogInstaller1.Source = EvictPowerSrv.LOG_SOURCE;
            eventLogInstaller1.Log = EvictPowerSrv.LOG_NAME;
            Installers.Add(eventLogInstaller1);

            EventLogInstaller eventLogInstaller2 = new EventLogInstaller();
            eventLogInstaller2.Source = LoopThread.LOG_SOURCE;
            eventLogInstaller2.Log = LoopThread.LOG_NAME;
            Installers.Add(eventLogInstaller2);

            // Start service after installing
            serviceInstaller1.AfterInstall += (sender, args) => new ServiceController(serviceInstaller1.ServiceName).Start();
        }
    }
}
