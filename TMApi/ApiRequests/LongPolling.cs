using ApiTypes;
using ApiTypes.Communication.LongPolling;

namespace TMApi.ApiRequests
{
    internal class LongPolling : BaseRequester
    {
        public bool IsPolling { get; private set; }
        private TimeSpan LongPollPeriod { get; }

        public event EventHandler<Notification>? StateUpdated;

        public LongPolling(TimeSpan longPollPeriod, RequestSender requester, Api api) : base(requester, api)
        {
            LongPollPeriod = longPollPeriod;
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
                    var notification = await Requester.LongPollAsync<Notification, LongPollingRequest>
                               (RequestHeaders.LongPoll, new LongPollingRequest(), LongPollPeriod);
                    if (notification != null)
                        StateUpdated?.Invoke(this, notification);
                }
            });
        }

    }
}
