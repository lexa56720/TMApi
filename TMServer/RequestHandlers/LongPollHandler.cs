using ApiTypes;
using ApiTypes.Communication.LongPolling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.RequestHandlers
{
    internal class LongPollHandler
    {
        public Notification GetUpdates(ApiData<LongPollingRequest> request)
        {
            var n = new Notification();
            return n;
        }
    }
}
