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

        public event EventHandler<Notification>? StateUpdated;

        private Notification? LastState;

        public LongPolling(RequestSender requester, TMApi api) : base(requester, api)
        {
        }

        public void Start()
        {
            if (IsPolling)
                return;

            IsPolling = true;
            RequestingLoop();
        }


        public void Stop()
        {
            if (!IsPolling)
                return;
            IsPolling = false;
        }


        private void RequestingLoop()
        {
            Task.Run(async () =>
            {
                while (IsPolling)
                {
                    LastState = await Requester.PostRequestAsync<Notification, LongPollingRequest>(RequestHeaders.LongPoll, new LongPollingRequest(), TimeSpan.MaxValue);
                    if (LastState != null)
                        StateUpdated?.Invoke(this, LastState);
                }
            });
        }

    }
}
