using ApiTypes;
using ApiTypes.Communication.LongPolling;

namespace TMApi.ApiRequests
{
    internal class LongPolling : BaseRequester
    {
        public bool IsPolling { get; private set; }

        public event EventHandler<Notification>? StateUpdated;

        private Notification? LastState;

        public LongPolling(RequestSender requester, Api api) : base(requester, api)
        {
        }

        public override void Dispose()
        {
            Stop();
            base.Dispose();
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
                    LastState = await Requester.PostAsync<Notification, LongPollingRequest>(RequestHeaders.LongPoll, new LongPollingRequest(), TimeSpan.MaxValue);
                    if (LastState != null)
                        StateUpdated?.Invoke(this, LastState);
                }
            });
        }

    }
}
