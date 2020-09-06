using System;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.ServiceProcess;
using System.Threading;

namespace EvictPowerSrv
{
    public partial class EvictPowerSrv : ServiceBase
    {
        private const String LOG_SOURCE = "Evict Power";
        private const String LOG_NAME = "Application";

        private EventLog log;
        private Thread workerThread;

        private static readonly HttpClient httpClient =
            new HttpClient();

        public EvictPowerSrv()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Init logging
            log = new EventLog();
            if (!EventLog.SourceExists(LOG_SOURCE))
            {
                EventLog.CreateEventSource(LOG_SOURCE, LOG_NAME);
            }
            log.Source = LOG_SOURCE;
            log.Log = LOG_NAME;

            // Init worker thread
            LoopThread thread = new LoopThread(httpClient, () =>
            {
                log.WriteEntry(Properties.strings.Service_Log_Shutdown, EventLogEntryType.Warning);
                return Win32Utils.ExitWindows();
            });
            workerThread = new Thread(thread.Start);
            workerThread.IsBackground = true;

            workerThread.Start();
        }

        protected override void OnStop()
        {
            workerThread.Abort();
            workerThread.Join();
        }
    }
}
