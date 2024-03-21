using ApiTypes;
using ApiTypes.Communication.LongPolling;

namespace TMApi.ApiRequests
{
    public class LongPolling : BaseRequester
    {
        private bool IsPolling { get; set; }
        private TimeSpan LongPollPeriod { get; }

        public event EventHandler<int[]>? NewMessages;
        public event EventHandler<int[]>? MessagesReaded;
        public event EventHandler<int[]>? NewChatInivites;
        public event EventHandler<int[]>? NewFriendRequests;
        public event EventHandler<int[]>? NewChats;
        public event EventHandler<int[]>? RemovedChats;
        public event EventHandler<int[]>? FriendsAdded;
        public event EventHandler<int[]>? FriendsRemoved;
        public event EventHandler<int[]>? ChatsChanged; 
        public event EventHandler<int[]>? RelatedUsersChanged;
        private readonly CancellationTokenSource TokenSource = new();
        internal LongPolling(TimeSpan longPollPeriod, RequestSender requester, Api api) : base(requester, api)
        {
            LongPollPeriod = longPollPeriod;
        }

        public override void Dispose()
        {
            Stop();
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
                var prevLongPollId = -1;
                while (IsPolling)
                {
                    var notification = await Requester.LongPollAsync<Notification, LongPollingRequest>
                                                      (new LongPollingRequest(prevLongPollId),
                                                       LongPollPeriod,
                                                       TokenSource.Token);
                    if (notification != null)
                    {
                        HandleNotification(notification);
                        prevLongPollId = notification.LongPollId;
                    }               
                }
            });
        }

        private void HandleNotification(Notification notification)
        {
            if (notification.FriendRequestIds.Length > 0)
                NewFriendRequests?.Invoke(this, notification.FriendRequestIds);

            if (notification.NewChatIds.Length > 0)
                NewChats?.Invoke(this, notification.NewChatIds);

            if (notification.RemovedChatIds.Length > 0)
                RemovedChats?.Invoke(this, notification.RemovedChatIds);

            if (notification.NewMessagesIds.Length > 0)
                NewMessages?.Invoke(this, notification.NewMessagesIds);

            if (notification.NewFriendsIds.Length > 0)
                FriendsAdded?.Invoke(this, notification.NewFriendsIds);

            if (notification.RemovedFriendsIds.Length > 0)
                FriendsRemoved?.Invoke(this, notification.RemovedFriendsIds);

            if (notification.ReadedMessagesIds.Length > 0)
                MessagesReaded?.Invoke(this, notification.ReadedMessagesIds);

            if (notification.ChatChangedIds.Length > 0)
                ChatsChanged?.Invoke(this, notification.ChatChangedIds);

            if (notification.RelatedUserChangedIds.Length > 0)
                RelatedUsersChanged?.Invoke(this, notification.RelatedUserChangedIds);

            if(notification.ChatInviteIds.Length > 0)
                NewChatInivites?.Invoke(this, notification.ChatInviteIds);
        }
    }
}
