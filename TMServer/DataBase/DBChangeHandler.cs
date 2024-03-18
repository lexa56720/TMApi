﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
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
            foreach (var member in entity.Members)
            {
                context.ChatUpdates.Add(new DBChatUpdate()
                {
                    ChatId = entity.Id,
                    UserId = member.Id
                });
            }
            return entity.Members.Select(m => m.Id);
        }

        private IEnumerable<int> HandleModifiedUser(DBUser user, TmdbContext context)
        {
            var chatMembers = user.Chats.SelectMany(c => c.Members.Where(m => m.Id != user.Id).Select(m => m.Id));
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
                nameof(DBChatUser) => HandleNewChatMember((DBChatUser)entity.Entity, context),
                _ => [],
            };
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
                                           .Members.Select(m => m.Id)
                                           .Where(id => id != message.AuthorId);

            //Добавление уведомлений в бд
            foreach (var member in chatMembers)
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
            var result = new List<int>();
            context.ChatListUpdates.Add(new DBChatListUpdate()
            {
                ChatId = chatId,
                IsAdded = isAdded,
                UserId = userId,
            });
            result.Add(userId);
            var chat = context.Chats.Single(c => c.Id == chatId);
            foreach (var member in chat.Members)
            {
                if (member.Id == userId)
                    continue;
                context.ChatUpdates.Add(new DBChatUpdate()
                {
                    ChatId = chatId,
                    UserId = member.Id,
                });
                result.Add(member.Id);
            }
            return result;
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
