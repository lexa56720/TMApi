﻿using ApiTypes.Communication.Friends;
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
        public (Notification notification, LongPollResponseInfo info) GetUpdates(int userId)
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

            var friendListSplitted = Split(friendListUpdates.GroupBy(f => f.FriendId));
            var chatListSplitted = Split(chatListUpdates.GroupBy(f => f.ChatId));

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

                RelatedUserChangedIds = relatedUsersChanged.Select(u => u.ProfileId).ToArray(),
                ChatChangedIds = chatsChanged.Select(c => c.ChatId).ToArray(),
                ChatInviteIds = chatInvites.Select(i => i.ChatInviteId).ToArray(),
            };

            return (notification, info);
        }

        private (int[] added, int[] removed) Split(IEnumerable<IGrouping<int, ListUpdate>> groupByForeignKey)
        {
            var added = new List<int>();
            var removed = new List<int>();


            foreach (var friendGroup in groupByForeignKey)
            {
                if (friendGroup.Count() != 1)
                    Add(friendGroup.MaxBy(f => f.Id));
                else
                    Add(friendGroup.First());
            }

            void Add(ListUpdate friendListUpdate)
            {
                if (friendListUpdate.IsAdded)
                    added.Add(friendListUpdate.TargetId);
                else
                    removed.Add(friendListUpdate.TargetId);
            }
            return (added.ToArray(), removed.ToArray());
        }

        private int[] ExtractIds(IEnumerable<Update> updates)
        {
            return updates.Select(u => u.Id).ToArray();
        }
        public void Clear(LongPollResponseInfo info)
        {
            LongPolling.ClearUpdatesByIds(info);
        }
        public bool IsHaveNotifications(int userId)
        {
            return LongPolling.IsHaveUpdates(userId);
        }
    }

}
