using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Tables.LongPolling;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase.Interaction
{
    public static class Changes
    {
        public static IEnumerable<int> HandleModifiedChat(DBChat entity, TmdbContext context)
        {
            var usersToNotify = GetUsersForChatUpdate(entity.Id, context);
            UpdateChatForUsers(entity.Id, usersToNotify, context);
            return usersToNotify;
        }
        public static IEnumerable<int> HandleModifiedUser(DBUser user, TmdbContext context)
        {

            user = context.Users.Include(u => u.FriendsTwo).ThenInclude(f => f.Sender)
                                .Include(u => u.FriendsOne).ThenInclude(f => f.Receiver)
                                .Include(u => u.Chats).ThenInclude(c => c.Members)
                                .Single(u => u.Id == user.Id);


            var chatMembers = user.Chats.SelectMany(c => c.Members.Where(m => m.Id != user.Id)
                                                                  .Select(m => m.Id));

            var invited = context.ChatInvites.Where(i => i.InviterId == user.Id)
                                             .Select(i => i.ToUserId);

            var requested = context.FriendRequests.Where(r => r.SenderId == user.Id)
                                                  .Select(r => r.ReceiverId);

            var result = new List<int>(chatMembers.Count() + invited.Count() + requested.Count());
            result.AddRange(chatMembers);
            result.AddRange(invited);
            result.AddRange(requested);

            return result;
        }

        public static IEnumerable<int> HandleRemovedFriend(DBFriend entity, TmdbContext context)
        {
            UpdateFriendList(false, entity.SenderId, entity.DestId, context);
            UpdateFriendList(false, entity.DestId, entity.SenderId, context);
            return [entity.DestId, entity.SenderId];
        }
        public static IEnumerable<int> HandleNewFriend(DBFriend entity, TmdbContext context)
        {
            UpdateFriendList(true, entity.SenderId, entity.DestId, context);
            UpdateFriendList(true, entity.DestId, entity.SenderId, context);
            return [entity.DestId, entity.SenderId];
        }
        public static void UpdateFriendList(bool isAdded, int userId, int friendId, TmdbContext context)
        {
            var update = context.FriendListUpdates.Where(f => f.UserId == userId && f.FriendId == friendId)
                                                  .SingleOrDefault();
            context.FriendListUpdates.Add(new DBFriendListUpdate()
            {
                FriendId = friendId,
                UserId = userId,
                IsAdded = isAdded,
            });
        }

        public static IEnumerable<int> HandleNewFriendRequest(DBFriendRequest entity, TmdbContext context)
        {
            context.FriendRequestUpdates.Add(new DBFriendRequestUpdate()
            {
                RequestId = entity.Id,
                UserId = entity.ReceiverId
            });
            return [entity.ReceiverId];
        }
        public static IEnumerable<int> HandleNewMessage(DBMessage message, TmdbContext context)
        {
            var chatMembers = context.Chats.Include(c => c.Members)
                                           .First(c => c.Id == message.DestinationId)
                                           .Members.Select(m => m.Id);

            //Добавление уведомлений в бд
            foreach (var member in chatMembers)
                if (message.IsSystem || member != message.AuthorId)
                    context.NewMessageUpdates.Add(new DBNewMessageUpdate()
                    {
                        MessageId = message.Id,
                        UserId = member
                    });
            return chatMembers;
        }
        public static IEnumerable<int> HandleMessageRead(DBUnreadMessage message, TmdbContext context)
        {
            context.MessageStatusUpdates.Add(new DBMessageStatusUpdate()
            {
                MessageId = message.MessageId,
                UserId = message.UserId,
            });
            return [message.UserId];
        }

        public static IEnumerable<int> HandleNewChatMember(DBChatUser entity, TmdbContext context)
        {
            return UpdateChatMembers(context, entity.ChatId, entity.UserId, true);
        }
        public static IEnumerable<int> HandleRemovedChatMember(DBChatUser entity, TmdbContext context)
        {
            return UpdateChatMembers(context, entity.ChatId, entity.UserId, false);
        }

        public static IEnumerable<int> UpdateChatMembers(TmdbContext context, int chatId, int userId, bool isAdded)
        {
            context.ChatListUpdates.Add(new DBChatListUpdate()
            {
                ChatId = chatId,
                IsAdded = isAdded,
                UserId = userId,
            });
            var usersToNotify = GetUsersForChatUpdate(chatId, context);
            UpdateChatForUsers(chatId, usersToNotify, context);
            usersToNotify.Add(userId);
            return usersToNotify;
        }
        public static List<int> GetUsersForChatUpdate(int chatId, TmdbContext context)
        {
            var usersToNotify = new List<int>();
            usersToNotify.AddRange(context.Chats.Include(c => c.Members)
                                                .Single(c => c.Id == chatId)
                                                .Members.Select(m => m.Id));

            usersToNotify.AddRange(context.ChatInvites.Where(i => i.ChatId == chatId)
                                                      .Select(i => i.ToUserId));

            return usersToNotify;
        }
        public static void UpdateChatForUsers(int chatId, IEnumerable<int> userIds, TmdbContext context)
        {
            foreach (var member in userIds)
            {
                context.ChatUpdates.Add(new DBChatUpdate()
                {
                    ChatId = chatId,
                    UserId = member,
                });
            }
        }

        public static IEnumerable<int> HandleNewChat(DBChat chat, TmdbContext context)
        {
            var members = chat.Members;
            foreach (var member in members)
                context.ChatListUpdates.Add(new DBChatListUpdate()
                {
                    ChatId = chat.Id,
                    UserId = member.Id,
                    IsAdded = true
                });
            return members.Select(m => m.Id);
        }

        public static IEnumerable<int> HandleNewChatInvite(DBChatInvite invite, TmdbContext context)
        {
            context.ChatInviteUpdates.Add(new DBChatInviteUpdate()
            {
                ChatInviteId = invite.Id,
                UserId = invite.ToUserId,
            });
            return [invite.ToUserId];
        }
    }
}
