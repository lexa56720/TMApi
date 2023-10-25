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
        public static void HandleChanges((EntityEntry, EntityState)[] entities)
        {
            foreach (var entity in entities)
            {
                if (entity.Item2 == EntityState.Added)
                {
                    var name = entity.Item1.Metadata.ClrType.Name;
                    HandleAddedEntity(name, entity.Item1);


                }
            }
        }

        private static void HandleAddedEntity(string className, EntityEntry entity)
        {
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
                                   .Members.Select(m => m.Id);
            foreach (var member in chatMembers)
                context.ChatUpdates.Add(new DBChatUpdate() 
                { 
                    MessageId = message.Id, 
                    UserId = member 
                });
            context.SaveChanges();
        }

    }
}
