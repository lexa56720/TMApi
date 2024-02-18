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
                switch (entity.state)
                {
                    case EntityState.Added:
                        var name = entity.entity.Metadata.ClrType.Name;
                        notifyList.AddRange(HandleAddedEntity(name, entity.entity, context));
                        break;
                }
            context.SaveChanges();
            return notifyList.Distinct().ToArray();
        }


        private IEnumerable<int> HandleAddedEntity(string className, EntityEntry entity, TmdbContext context)
        {
            return className switch
            {
                nameof(DBMessage) => HandleNewMessage((DBMessage)entity.Entity, context),
                _ => [],
            };
        }

        private IEnumerable<int> HandleNewMessage(DBMessage message, TmdbContext context)
        {

            var chatMembers = context.Chats.First(c => c.Id == message.DestinationId)
                                   .Members.Select(m => m.Id)
                                   .Where(id => id != message.AuthorId);

            //Добавление уведомлений в бд
            foreach (var member in chatMembers)
                context.ChatUpdates.Add(new DBChatUpdate()
                {
                    MessageId = message.Id,
                    UserId = member
                });

            return chatMembers;
        }

        private void NotifyUser(int id)
        {
            UpdateForUser?.Invoke(null, id);
        }
    }
}
