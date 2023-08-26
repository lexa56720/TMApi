using ApiTypes;
using ApiTypes.LongPolling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMApi.ApiRequests
{
    internal class LongPolling : BaseRequester
    {
        public bool IsPolling { get; private set; }

        public event EventHandler<Notification>? NewEvents;

        private Notification? LastState;

        public LongPolling(RequestSender requester, TMApi api) : base(requester, api)
        {
        }

        public void Start()
        {
            if (IsPolling)
                return;

            Requester.PostRequestAsync<Notification,LongPollingRequest>(RequestHeaders.LongPoll, new LongPollingRequest(), TimeSpan.MaxValue);
        }


        public void Stop()
        {
            if (!IsPolling)
                return;
        }

    }
}
