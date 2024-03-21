using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TMServer.DataBase.Tables;
using TMServer.DataBase.Tables.LongPolling;
using TMServer.DataBase.Interaction;
using ApiTypes.Communication.Chats;

namespace TMServer.DataBase
{
    public class DBChangeHandler
    {
        public event EventHandler<int>? UpdateForUser;
        public bool IsUpdateTracked => UpdateForUser != null;

        public void HandleChanges((EntityEntry entity, EntityState state)[] entities)
        {
            if (!IsUpdateTracked)
                return;

            var userIds = GetAffectedUsers(entities);
            foreach (var userId in userIds)
                NotifyUser(userId);
        }

        private int[] GetAffectedUsers((EntityEntry entity, EntityState state)[] entities)
        {
            using var context = new TmdbContext();
            var notifyList = new List<int>();
            foreach (var entity in entities)
            {
                var className = entity.entity.Metadata.ClrType.Name;

                switch (entity.state)
                {
                    case EntityState.Added:
                        notifyList.AddRange(HandleAddedEntity(className, entity.entity, context));
                        break;
                    case EntityState.Deleted:
                        notifyList.AddRange(HandleDeletedEntity(className, entity.entity, context));
                        break;
                    case EntityState.Modified:
                        notifyList.AddRange(HandleModifiedEntity(className, entity.entity, context));
                        break;
                }
            }
            context.SaveChanges();
            return notifyList.Distinct().ToArray();
        }
        private IEnumerable<int> HandleModifiedEntity(string className, EntityEntry entity, TmdbContext context)
        {
            return className switch
            {
                nameof(DBUser) => HandleModifiedUser((DBUser)entity.Entity, context),
                nameof(DBChat) => HandleModifiedChat((DBChat)entity.Entity, context),
                _ => [],
            };
        }


        private IEnumerable<int> HandleAddedEntity(string className, EntityEntry entity, TmdbContext context)
        {
            return className switch
            {
                nameof(DBMessage) => HandleNewMessage((DBMessage)entity.Entity, context),
                nameof(DBFriendRequest) => HandleNewFriendRequest((DBFriendRequest)entity.Entity, context),
                nameof(DBFriend) => HandleNewFriend((DBFriend)entity.Entity, context),
                nameof(DBChat) => HandleNewChat((DBChat)entity.Entity, context),
                nameof(DBChatUser) => HandleNewChatMember((DBChatUser)entity.Entity, context),
                _ => [],
            };
        }

        private IEnumerable<int> HandleDeletedEntity(string className, EntityEntry entity, TmdbContext context)
        {
            return className switch
            {
                nameof(DBFriend) => HandleRemovedFriend((DBFriend)entity.Entity, context),
                nameof(DBUnreadMessage) => HandleMessageRead((DBUnreadMessage)entity.Entity, context),
                nameof(DBChatUser) => HandleRemovedChatMember((DBChatUser)entity.Entity, context),
                _ => [],
            };
        }

        private IEnumerable<int> HandleModifiedChat(DBChat entity, TmdbContext context)
        {
            var usersToNotify = GetUsersForChatUpdate(entity.Id, context);
            UpdateChatForUsers(entity.Id, usersToNotify, context);
            return usersToNotify;
        }

        private IEnumerable<int> HandleModifiedUser(DBUser user, TmdbContext context)
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


        private IEnumerable<int> HandleRemovedFriend(DBFriend entity, TmdbContext context)
        {
            UpdateFriendList(false, entity.SenderId, entity.DestId, context);
            UpdateFriendList(false, entity.DestId, entity.SenderId, context);
            return [entity.DestId, entity.SenderId];
        }
        private IEnumerable<int> HandleNewFriend(DBFriend entity, TmdbContext context)
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
            });
        }

        private IEnumerable<int> HandleNewFriendRequest(DBFriendRequest entity, TmdbContext context)
        {
            context.FriendRequestUpdates.Add(new DBFriendRequestUpdate()
            {
                RequestId = entity.Id,
                UserId = entity.ReceiverId
            });
            return [entity.ReceiverId];
        }
        private IEnumerable<int> HandleNewMessage(DBMessage message, TmdbContext context)
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
        private IEnumerable<int> HandleMessageRead(DBUnreadMessage message, TmdbContext context)
        {
            context.MessageStatusUpdates.Add(new DBMessageStatusUpdate()
            {
                MessageId = message.MessageId,
                UserId = message.UserId,
            });
            return [message.UserId];
        }

        private IEnumerable<int> HandleNewChatMember(DBChatUser entity, TmdbContext context)
        {
            return UpdateChatMembers(context, entity.ChatId, entity.UserId, true);
        }
        private IEnumerable<int> HandleRemovedChatMember(DBChatUser entity, TmdbContext context)
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
            });
            var usersToNotify = GetUsersForChatUpdate(chatId, context);
            UpdateChatForUsers(chatId, usersToNotify, context);
            usersToNotify.Add(userId);
            return usersToNotify;
        }
        private List<int> GetUsersForChatUpdate(int chatId, TmdbContext context)
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
            foreach (var member in userIds)
            {
                context.ChatUpdates.Add(new DBChatUpdate()
                {
                    ChatId = chatId,
                    UserId = member,
                });
            }
        }


        private IEnumerable<int> HandleNewChat(DBChat chat, TmdbContext context)
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

        private void NotifyUser(int id)
        {
            UpdateForUser?.Invoke(null, id);
        }
    }
}
