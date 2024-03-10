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
                }
            }
            context.SaveChanges();
            return notifyList.Distinct().ToArray();
        }

        private IEnumerable<int> HandleDeletedEntity(string className, EntityEntry entity, TmdbContext context)
        {
            return className switch
            {
                nameof(DBFriend) => HandleRemovedFriend((DBFriend)entity.Entity, context),
                nameof(DBUnreadedMessage) => HandleMessageRead((DBUnreadedMessage)entity.Entity, context),
                _ => [],
            };
        }

        private IEnumerable<int> HandleMessageRead(DBUnreadedMessage message, TmdbContext context)
        {
            var chatMembers = context.Messages.Include(m => m.Destination)
                                              .ThenInclude(c => c.Members)
                                              .First(m => m.Id == message.MessageId)
                                              .Destination.Members.Select(m => m.Id);
            foreach (var member in chatMembers)
                context.MessageStatusUpdates.Add(new DBMessageStatusUpdate()
                {
                    MessageId = message.MessageId,
                    UserId = member,
                });
            return chatMembers;
        }

        private IEnumerable<int> HandleAddedEntity(string className, EntityEntry entity, TmdbContext context)
        {
            return className switch
            {
                nameof(DBMessage) => HandleNewMessage((DBMessage)entity.Entity, context),
                nameof(DBFriendRequest) => HandleNewFriendRequest((DBFriendRequest)entity.Entity, context),
                nameof(DBFriend) => HandleNewFriend((DBFriend)entity.Entity, context),
                nameof(DBChat) => HandleNewChat((DBChat)entity.Entity, context),

                _ => [],
            };
        }
        private IEnumerable<int> HandleRemovedFriend(DBFriend entity, TmdbContext context)
        {
            UpdateFriendList(false, entity.SenderId, entity.DestId, context);
            return [entity.DestId, entity.SenderId];
        }
        private IEnumerable<int> HandleNewFriend(DBFriend entity, TmdbContext context)
        {
            UpdateFriendList(true, entity.SenderId, entity.DestId, context);
            return [entity.DestId, entity.SenderId];
        }
        private void UpdateFriendList(bool isAdded, int userId, int friendId, TmdbContext context)
        {
            context.FriendListUpdates.Add(new DBFriendListUpdate()
            {
                FriendId = userId,
                UserId = friendId,
                IsAdded = isAdded,
            });

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
                                           .Members.Select(m => m.Id)
                                           .Where(id => id != message.AuthorId);

            //Добавление уведомлений в бд
            foreach (var member in chatMembers)
                context.NewMessages.Add(new DBNewMessages()
                {
                    MessageId = message.Id,
                    UserId = member
                });
            return chatMembers;
        }

        private IEnumerable<int> HandleNewChat(DBChat chat, TmdbContext context)
        {
            var members = chat.Members;
            foreach (var member in members)
                context.ChatUpdates.Add(new DBChatUpdate()
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
