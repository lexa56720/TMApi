using ApiTypes.Communication.LongPolling;
using TMServer.DataBase.Interaction;
using TMServer.DataBase.Tables.LongPolling;
using TMServer.ServerComponent.LongPolling;

namespace TMServer.RequestHandlers
{
    internal static class LongPollHandler
    {
        private static readonly Random Random = new();
        public static (Notification notification, LongPollResponseInfo info) GetUpdates(int userId)
        {
            var id = Random.Next();


            var newMessages = LongPolling.GetMessageUpdate(userId);
            var readedMessages = LongPolling.GetMessagesWithUpdatedStatus(userId);

            var friendRequests = LongPolling.GetFriendRequestUpdates(userId);
            var friendListUpdates = LongPolling.GetFriendListUpdates(userId);

            var relatedUsersChanged = LongPolling.GetRelatedUsersUpdates(userId);

            var chatListUpdates = LongPolling.GetChatListUpdates(userId);
            var chatsChanged = LongPolling.GetChatUpdates(userId);
            var chatInvites = LongPolling.GetChatInvites(userId);


            var info = new LongPollResponseInfo()
            {
                Id = id,

                NewMessages = ExtractIds(newMessages),
                ReadedMessages = ExtractIds(readedMessages),
                FriendRequests = ExtractIds(friendRequests),
                FriendListUpdates = ExtractIds(friendListUpdates),
                RelatedUsersChanged = ExtractIds(relatedUsersChanged),
                ChatListUpdates = ExtractIds(chatListUpdates),
                ChatsChanged = ExtractIds(chatsChanged),
                ChatInvites = ExtractIds(chatInvites),
            };

            var notification = new Notification()
            {
                LongPollId = id,

                NewMessagesIds = newMessages.Select(m => m.MessageId).ToArray(),
                ReadedMessagesIds = readedMessages.Select(m => m.MessageId).ToArray(),

                FriendRequestIds = friendRequests.Select(r => r.RequestId).ToArray(),

                NewFriendsIds = friendListUpdates.Where(f => f.IsAdded).Select(f => f.FriendId).ToArray(),
                RemovedFriendsIds = friendListUpdates.Where(f => !f.IsAdded).Select(f => f.FriendId).ToArray(),

                NewChatIds = chatListUpdates.Where(c => c.IsAdded).Select(c => c.ChatId).ToArray(),
                RemovedChatIds = chatListUpdates.Where(c => !c.IsAdded).Select(c => c.ChatId).ToArray(),

                RelatedUserChangedIds = relatedUsersChanged.Select(u => u.ProfileId).ToArray(),
                ChatChangedIds = chatsChanged.Select(c => c.ChatId).ToArray(),
                ChatInviteIds = chatInvites.Select(i => i.ChatInviteId).ToArray(),
            };

            return (notification, info);
        }

        private static int[] ExtractIds(IEnumerable<DBUpdate> updates)
        {
            return updates.Select(u => u.Id).ToArray();
        }
        public static void Clear(LongPollResponseInfo info)
        {
            LongPolling.ClearUpdatesByIds(info);
        }
        public static bool IsHaveNotifications(int userId)
        {
            return LongPolling.IsHaveUpdates(userId);
        }
    }

}
