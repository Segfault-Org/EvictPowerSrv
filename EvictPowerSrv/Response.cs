using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvictPowerSrv
{
    class Response
    {
        public int DocumentIncarnation { get; set; }
        public IList<Event> Events { get; set; }

        public class Event
        {
            public String EventId { get; set; }

            public String EventType { get; set; }

            public String ResourceType { get; set; }

            public IList<String> Resources { get; set; }

            public String EventStatus { get; set; }

            public String NotBefore { get; set; }

            public String Description { get; set; }

            public String EventSource { get; set; }
        }
    }
}
