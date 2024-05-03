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

        public async Task HandleChanges((EntityEntry entity, EntityState state)[] entities)
        {
            if (!IsUpdateTracked)
                return;
            //Получение списка пользователей, которых затронуло изменение
            var userIds = await GetAffectedUsers(entities);

            //Уведомление каждого из пользователй
            foreach (var userId in userIds)
                NotifyUser(userId);
        }

        private async Task<int[]> GetAffectedUsers((EntityEntry entity, EntityState state)[] entities)
        {
            using var context = new TmdbContext();
            var notifyList = new List<int>();
            foreach (var entity in entities)
            {
                var className = entity.entity.Metadata.ClrType.Name;

                switch (entity.state)
                {
                    case EntityState.Added:
                        notifyList.AddRange(await HandleAddedEntity(className, entity.entity, context));
                        break;
                    case EntityState.Deleted:
                        notifyList.AddRange(await HandleDeletedEntity(className, entity.entity, context));
                        break;
                    case EntityState.Modified:
                        notifyList.AddRange(await HandleModifiedEntity(className, entity.entity, context));
                        break;
                }
            }
            await context.SaveChangesAsync();
            return notifyList.Distinct().ToArray();
        }
        private async Task<IEnumerable<int>> HandleModifiedEntity(string className, EntityEntry entity, TmdbContext context)
        {
            return className switch
            {
                nameof(DBUser) => await Changes.HandleModifiedUser((DBUser)entity.Entity, context),
                nameof(DBChat) => await Changes.HandleModifiedChat((DBChat)entity.Entity, context),
                _ => [],
            };
        }


        private async Task<IEnumerable<int>> HandleAddedEntity(string className, EntityEntry entity, TmdbContext context)
        {
            return className switch
            {
                nameof(DBMessage) => await Changes.HandleNewMessage((DBMessage)entity.Entity, context),
                nameof(DBFriendRequest) => await Changes.HandleNewFriendRequest((DBFriendRequest)entity.Entity, context),
                nameof(DBFriend) => await Changes.HandleNewFriend((DBFriend)entity.Entity, context),
                nameof(DBChat) => await Changes.HandleNewChat((DBChat)entity.Entity, context),
                nameof(DBChatUser) => await Changes.HandleNewChatMember((DBChatUser)entity.Entity, context),
                nameof(DBChatInvite) => await Changes.HandleNewChatInvite((DBChatInvite)entity.Entity, context),
                _ => [],
            };
        }

        private async Task<IEnumerable<int>> HandleDeletedEntity(string className, EntityEntry entity, TmdbContext context)
        {
            return className switch
            {
                nameof(DBFriend) => await Changes.HandleRemovedFriend((DBFriend)entity.Entity, context),
                nameof(DBUnreadMessage) => await Changes.HandleMessageRead((DBUnreadMessage)entity.Entity, context),
                nameof(DBChatUser) => await Changes.HandleRemovedChatMember((DBChatUser)entity.Entity, context),
                _ => [],
            };
        }

        private void NotifyUser(int id)
        {
            UpdateForUser?.Invoke(null, id);
        }
    }
}
