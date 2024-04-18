using ApiTypes.Communication.Friends;
using ApiTypes.Communication.LongPolling;
using System;
using TMServer.DataBase.Interaction;
using TMServer.DataBase.Tables.LongPolling;
using TMServer.ServerComponent.LongPolling;

namespace TMServer.RequestHandlers
{
    public class LongPollHandler
    {
        private readonly Random Random = new();
        private readonly LongPolling LongPolling;

        public LongPollHandler(LongPolling longPolling)
        {
            LongPolling = longPolling;
        }
        public async Task<(Notification notification, LongPollResponseInfo info)> GetUpdates(int userId)
        {
            var id = Random.Next();

            var newMessages =await LongPolling.GetMessageUpdate(userId);
            var readedMessages = await LongPolling.GetMessagesWithUpdatedStatus(userId);

            var friendRequests = await LongPolling.GetFriendRequestUpdates(userId);
            var friendListUpdates = await LongPolling.GetFriendListUpdates(userId);
            var onlineUpdates = await LongPolling.GetOnlineUpdates(userId);
            var relatedUsersChanged = await LongPolling.GetRelatedUsersUpdates(userId);

            var chatListUpdates = await LongPolling.GetChatListUpdates(userId);
            var chatsChanged = await LongPolling.GetChatUpdates(userId);
            var chatInvites = await LongPolling.GetChatInvites(userId);

            var info = new LongPollResponseInfo()
            {
                Id = id,
                OnlineUpdates = ExtractIds(onlineUpdates),
                NewMessages = ExtractIds(newMessages),
                ReadedMessages = ExtractIds(readedMessages),
                FriendRequests = ExtractIds(friendRequests),
                FriendListUpdates = ExtractIds(friendListUpdates),
                RelatedUsersChanged = ExtractIds(relatedUsersChanged),
                ChatListUpdates = ExtractIds(chatListUpdates),
                ChatsChanged = ExtractIds(chatsChanged),
                ChatInvites = ExtractIds(chatInvites),
            };

            var friendListSplitted = Split(friendListUpdates);
            var chatListSplitted = Split(chatListUpdates);
            var onlineSplitted = Split(onlineUpdates);

            var notification = new Notification()
            {
                LongPollId = id,

                NewMessagesIds = newMessages.Select(m => m.MessageId).ToArray(),
                ReadedMessagesIds = readedMessages.Select(m => m.MessageId).ToArray(),

                FriendRequestIds = friendRequests.Select(r => r.RequestId).ToArray(),

                NewFriendsIds = friendListSplitted.added,
                RemovedFriendsIds = friendListSplitted.removed,

                NewChatIds = chatListSplitted.added,
                RemovedChatIds = chatListSplitted.removed,

                RelatedUserOnlineIds = onlineSplitted.added,
                RelatedUserOfflineIds = onlineSplitted.removed,

                RelatedUserChangedIds = relatedUsersChanged.Select(u => u.ProfileId).ToArray(),
                ChatChangedIds = chatsChanged.Select(c => c.ChatId).ToArray(),
                ChatInviteIds = chatInvites.Select(i => i.ChatInviteId).ToArray(),
            };

            return (notification, info);
        }

        private (int[] added, int[] removed) Split(IEnumerable<ListUpdate> list)
        {
            var added = new List<int>();
            var removed = new List<int>();

            var groupped = list.GroupBy(l => l.TargetId)
                               .Select(g => g.OrderBy(e => e.Date)
                                             .Last());
            foreach (var group in groupped)
            {
                if (group == null)
                    continue;
                if (group.IsAdded)
                    added.Add(group.TargetId);
                else
                    removed.Add(group.TargetId);
            }
            return (added.ToArray(), removed.ToArray());
        }

        private int[] ExtractIds(IEnumerable<Update> updates)
        {
            return updates.Select(u => u.Id).ToArray();
        }
        public async void Clear(LongPollResponseInfo info)
        {
          await  LongPolling.ClearUpdatesByIds(info);
        }
    }

}
