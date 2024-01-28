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
    internal static class DBChangeHandler
    {
        public static event EventHandler<int>? UpdateForUser;
        public static void HandleChanges((EntityEntry entity, EntityState state)[] entities)
        {
            foreach (var entity in entities)
            {
                switch (entity.state)
                {
                    case EntityState.Added:
                        var name = entity.entity.Metadata.ClrType.Name;
                        HandleAddedEntity(name, entity.entity);
                        break;
                }
            }
        }

        private static void HandleAddedEntity(string className, EntityEntry entity)
        {
            //Уведомление юзера об обнове в бд на его айди
            if (entity.Metadata.ClrType.BaseType == typeof(DBUpdate))
            {
                NotifyUser((DBUpdate)entity.Entity);
                return;
            }

            switch (className)
            {
                case nameof(DBMessage):
                    HandleNewMessage((DBMessage)entity.Entity);
                    break;
            }
        }

        private static void HandleNewMessage(DBMessage message)
        {
            using var context = new TmdbContext();

            var chatMembers = Chats.GetChat(message.DestinationId)
                                   .Members.Select(m => m.Id)
                                   .Where(id=>id!=message.AuthorId);

            //Добавление уведомлений в бд
            foreach (var member in chatMembers)
                context.ChatUpdates.Add(new DBChatUpdate()
                {
                    MessageId = message.Id,
                    UserId = member
                });
            context.SaveChanges();
        }


        private static void NotifyUser(DBUpdate update)
        {
            UpdateForUser?.Invoke(null, update.UserId);
        }

    }
}
