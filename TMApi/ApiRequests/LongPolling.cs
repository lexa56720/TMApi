using ApiTypes;
using ApiTypes.Communication.LongPolling;

namespace TMApi.ApiRequests
{
    public class LongPolling : BaseRequester
    {
        private bool IsPolling { get; set; }
        private TimeSpan LongPollPeriod { get; }

        public event EventHandler<int[]>? NewMessages;
        public event EventHandler<int[]>? NewFriendRequests;
        public event EventHandler<int[]>? NewChats;
        public event EventHandler<int[]>? FriendsAdded;
        public event EventHandler<int[]>? FriendsRemoved;

        private readonly CancellationTokenSource TokenSource = new();
        internal LongPolling(TimeSpan longPollPeriod, RequestSender requester, Api api) : base(requester, api)
        {
            LongPollPeriod = longPollPeriod;
        }

        public override void Dispose()
        {
            Stop();
            NewMessages = null;
            NewFriendRequests = null;
            NewChats = null;
            FriendsAdded = null;
            FriendsRemoved = null;
            TokenSource.Cancel();
            TokenSource.Dispose();
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
                    var notification = await Requester.LongPollAsync<Notification, LongPollingRequest>(new LongPollingRequest(),
                                                                                                       LongPollPeriod, 
                                                                                                       TokenSource.Token);
                    if (notification != null)
                        HandleNotification(notification);
                }
            });
        }

        private void HandleNotification(Notification notification)
        {
            if (notification.FriendRequestIds.Length > 0)
                NewFriendRequests?.Invoke(this, notification.FriendRequestIds);

            if (notification.NewChats.Length > 0)
                NewChats?.Invoke(this, notification.NewChats);

            if (notification.MessagesIds.Length > 0)
                NewMessages?.Invoke(this, notification.MessagesIds);

            if (notification.NewFriends.Length > 0)
                FriendsAdded?.Invoke(this, notification.NewFriends);

            if (notification.RemovedFriends.Length > 0)
                FriendsRemoved?.Invoke(this, notification.RemovedFriends);
        }
    }
}
