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
    public class Changes
    {
        public IEnumerable<int> HandleModifiedChat(DBChat entity, TmdbContext context)
        {
            var usersToNotify = GetUsersForChatUpdate(entity.Id, context);
            UpdateChatForUsers(entity.Id, usersToNotify, context);
            return usersToNotify;
        }
        public IEnumerable<int> HandleModifiedUser(DBUser user, TmdbContext context)
        {
            return Users.GetAllRelatedUsers(user.Id);
        }

        public IEnumerable<int> HandleRemovedFriend(DBFriend entity, TmdbContext context)
        {
            UpdateFriendList(false, entity.SenderId, entity.DestId, context);
            UpdateFriendList(false, entity.DestId, entity.SenderId, context);
            return [entity.DestId, entity.SenderId];
        }
        public IEnumerable<int> HandleNewFriend(DBFriend entity, TmdbContext context)
        {
            UpdateFriendList(true, entity.SenderId, entity.DestId, context);
            UpdateFriendList(true, entity.DestId, entity.SenderId, context);
            return [entity.DestId, entity.SenderId];
        }
        private void UpdateFriendList(bool isAdded, int userId, int friendId, TmdbContext context)
        {
            var update = context.FriendListUpdates.Where(f => f.UserId == userId && f.FriendId == friendId)
                                                  .SingleOrDefault();
            context.FriendListUpdates.Add(new DBFriendListUpdate()
            {
                FriendId = friendId,
                UserId = userId,
                IsAdded = isAdded,
                Date= DateTime.UtcNow,
            });
        }

        public IEnumerable<int> HandleNewFriendRequest(DBFriendRequest entity, TmdbContext context)
        {
            context.FriendRequestUpdates.Add(new DBFriendRequestUpdate()
            {
                RequestId = entity.Id,
                UserId = entity.ReceiverId
            });
            return [entity.ReceiverId];
        }
        public IEnumerable<int> HandleNewMessage(DBMessage message, TmdbContext context)
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
        public IEnumerable<int> HandleMessageRead(DBUnreadMessage message, TmdbContext context)
        {
            context.MessageStatusUpdates.Add(new DBMessageStatusUpdate()
            {
                MessageId = message.MessageId,
                UserId = message.UserId,
            });
            return [message.UserId];
        }

        public IEnumerable<int> HandleNewChatMember(DBChatUser entity, TmdbContext context)
        {
            return UpdateChatMembers(context, entity.ChatId, entity.UserId, true);
        }
        public IEnumerable<int> HandleRemovedChatMember(DBChatUser entity, TmdbContext context)
        {
            return UpdateChatMembers(context, entity.ChatId, entity.UserId, false);
        }
        private IEnumerable<int> UpdateChatMembers(TmdbContext context, int chatId, int userId, bool isAdded)
        {
            context.ChatListUpdates.Add(new DBChatListUpdate()
            {
                ChatId = chatId,
                IsAdded = isAdded,
                UserId = userId,
                Date = DateTime.UtcNow,
            });
            var usersToNotify = GetUsersForChatUpdate(chatId, context);
            UpdateChatForUsers(chatId, usersToNotify, context);
            usersToNotify.Add(userId);
            return usersToNotify;
        }


        public List<int> GetUsersForChatUpdate(int chatId, TmdbContext context)
        {
            var usersToNotify = new List<int>();
            usersToNotify.AddRange(context.Chats.Include(c => c.Members)
                                                .Single(c => c.Id == chatId)
                                                .Members.Select(m => m.Id));

            usersToNotify.AddRange(context.ChatInvites.Where(i => i.ChatId == chatId)
                                                      .Select(i => i.ToUserId));

            return usersToNotify;
        }
        private void UpdateChatForUsers(int chatId, IEnumerable<int> userIds, TmdbContext context)
        {
            foreach (var userId in userIds)
            {
                context.ChatUpdates.Add(new DBChatUpdate()
                {
                    ChatId = chatId,
                    UserId = userId,
                });
            }
        }

        public IEnumerable<int> HandleNewChat(DBChat chat, TmdbContext context)
        {
            var members = chat.Members;
            foreach (var member in members)
                context.ChatListUpdates.Add(new DBChatListUpdate()
                {
                    ChatId = chat.Id,
                    UserId = member.Id,
                    IsAdded = true,
                    Date = DateTime.UtcNow,
                });
            return members.Select(m => m.Id);
        }
        public IEnumerable<int> HandleNewChatInvite(DBChatInvite invite, TmdbContext context)
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
