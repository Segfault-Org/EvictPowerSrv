using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Http;
using System.Text.Json;
using System.Globalization;

namespace EvictPowerSrv
{
    public delegate bool Shutdown();
    class LoopThread
    {
        public const String LOG_SOURCE = "Evict Power Loop";
        public const String LOG_NAME = "Application";

        private EventLog log;
        private readonly Shutdown shutdownCallback;
        private readonly HttpClient client;

        public LoopThread(HttpClient client, Shutdown shutdownCallback)
        {
            this.client = client;
            this.shutdownCallback = shutdownCallback;
        }

        public void Start()
        {
            log = new EventLog();
            log.Source = LOG_SOURCE;
            log.Log = LOG_NAME;

            while (true)
            {
                Status s = CheckStatus();
                switch (s)
                {
                    case Status.OK:
                        Thread.Sleep(5000);
                        break;
                    case Status.ERROR:
                        log.WriteEntry(Properties.strings.Loop_Log_Error_CheckStatus, 
                            EventLogEntryType.Error);
                        Thread.Sleep(7000);
                        break;
                    case Status.NEED_SHUTDOWN:
                        log.WriteEntry(Properties.strings.Loop_Log_Shutdown,
                            EventLogEntryType.Warning);
                        if (!shutdownCallback())
                        {
                            log.WriteEntry("Cannot shutdown.", EventLogEntryType.Error);
                            break;
                        } else
                        {
                            // Prevent further loops
                            return;
                        }
                }
            }
        }

        /// <summary>
        /// Check whether it is required to shutdown.
        /// </summary>
        /// <returns></returns>
        private Status CheckStatus()
        {
            HttpRequestMessage req = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri =
                    new Uri("http://169.254.169.254/metadata/scheduledevents?api-version=2019-08-01"),
                Headers =
                {
                    {
                        "Metadata", "true"
                    }
                }
            };
            try
            {
                HttpResponseMessage resp = client.SendAsync(req).Result;
                if (!resp.IsSuccessStatusCode)
                {
                    log.WriteEntry(String.Format(Properties.strings.Loop_Log_CheckStatus_Unexpected_HTTP_Code, resp.StatusCode),
                        EventLogEntryType.Error);
                    return Status.ERROR;
                }

                Response eventResp = 
                    JsonSerializer.Deserialize<Response>(resp.Content.ReadAsStringAsync().Result);
                if (eventResp.Events.Count(item =>
                {
                    if (item.EventType == "Preempt")
                    {
                        log.WriteEntry(
                            String.Format(Properties.strings.Loop_Log_CheckStatus_Preempt,
                            item.EventStatus,
                            item.NotBefore,
                            item.EventSource),
                            EventLogEntryType.Information);
                        return true;
                    }
                    return false;
                }) > 0)
                    return Status.NEED_SHUTDOWN;
                else
                    return Status.OK;
            }
            catch (ThreadAbortException)
            {
                return Status.OK;
            }
            catch (Exception e)
            {
                log.WriteEntry(String.Format(Properties.strings.Loop_Log_CheckStatus_Error, e.Message, e.StackTrace),
                    EventLogEntryType.Error);
                return Status.ERROR;
            }
        }

        private enum Status
        {
            OK,
            NEED_SHUTDOWN,
            ERROR
        }
    }
}
