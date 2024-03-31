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
using TMServer.DataBase;

namespace TMServer.RequestHandlers
{
    public class ChangeHandler
    {
        private readonly Changes Changes;

        public event EventHandler<int>? UpdateForUser;
        public bool IsUpdateTracked => UpdateForUser != null;

        public ChangeHandler(Changes changes) 
        {
            Changes = changes;
        }

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
                nameof(DBUser) => Changes.HandleModifiedUser((DBUser)entity.Entity, context),
                nameof(DBChat) => Changes.HandleModifiedChat((DBChat)entity.Entity, context),
                _ => [],
            };
        }


        private IEnumerable<int> HandleAddedEntity(string className, EntityEntry entity, TmdbContext context)
        {
            return className switch
            {
                nameof(DBMessage) => Changes.HandleNewMessage((DBMessage)entity.Entity, context),
                nameof(DBFriendRequest) => Changes.HandleNewFriendRequest((DBFriendRequest)entity.Entity, context),
                nameof(DBFriend) => Changes.HandleNewFriend((DBFriend)entity.Entity, context),
                nameof(DBChat) => Changes.HandleNewChat((DBChat)entity.Entity, context),
                nameof(DBChatUser) => Changes.HandleNewChatMember((DBChatUser)entity.Entity, context),
                nameof(DBChatInvite) => Changes.HandleNewChatInvite((DBChatInvite)entity.Entity, context),
                _ => [],
            };
        }

        private IEnumerable<int> HandleDeletedEntity(string className, EntityEntry entity, TmdbContext context)
        {
            return className switch
            {
                nameof(DBFriend) =>Changes.HandleRemovedFriend((DBFriend)entity.Entity, context),
                nameof(DBUnreadMessage) => Changes.HandleMessageRead((DBUnreadMessage)entity.Entity, context),
                nameof(DBChatUser) => Changes.HandleRemovedChatMember((DBChatUser)entity.Entity, context),
                _ => [],
            };
        }

        private void NotifyUser(int id)
        {
            UpdateForUser?.Invoke(null, id);
        }
    }
}
