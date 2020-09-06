using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Text;
using System.Diagnostics;

namespace EvictPowerSrv
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(String[] args)
        {
            ServiceBase.Run(new EvictPowerSrv());
        }
    }
}
