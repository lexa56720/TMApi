using ApiTypes.Communication.Messages;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase.Interaction
{
    internal static class Messages
    {
        public static DBMessage AddMessage(int authorId, string content, int destinationId)
        {
            using var db = new TmdbContext();
            var message = new DBMessage()
            {
                AuthorId = authorId,
                DestinationId = destinationId,
                Content = content,
                IsSystem = false,
                SendTime = DateTime.UtcNow,
            };
            db.Messages.Add(message);
            db.SaveChanges(true);

            return message;
        }
        public static void AddSystemMessage(int chatId, int executorId, ActionKind kind, IEnumerable<int> targetIds)
        {
            using var db = new TmdbContext();
            foreach (var targetId in targetIds)
            {
                var message = new DBMessage()
                {
                    AuthorId = executorId,
                    DestinationId = chatId,
                    Content = string.Empty,
                    IsSystem = true,
                    SendTime = DateTime.UtcNow,
                };
                var action = new DBMessageAction()
                {
                    ExecutorId = executorId,
                    TargetId = targetId,
                    Kind = kind,
                    Message = message,
                };
                db.Messages.Add(message);
                db.MessageActions.Add(action);
            }
            db.SaveChanges(true);
        }
        public static void AddSystemMessage(int chatId, int executorId, ActionKind kind)
        {
            using var db = new TmdbContext();
            var message = new DBMessage()
            {
                AuthorId = executorId,
                DestinationId = chatId,
                Content = string.Empty,
                IsSystem = true,
                SendTime = DateTime.UtcNow,
            };
            var action = new DBMessageAction()
            {
                ExecutorId = executorId,
                Kind = kind,
                Message = message,
            };
            db.Messages.Add(message);
            db.MessageActions.Add(action);
            db.SaveChanges(true);
        }
        public static bool AddToUnread(int messageId, int chatId)
        {
            using var db = new TmdbContext();
            var members = db.Chats.Include(c => c.Members)
                                  .First(c => c.Id == chatId).Members;

            foreach (var member in members)
                db.UnreadMessages.Add(new DBUnreadMessage()
                {
                    UserId = member.Id,
                    MessageId = messageId,
                });
            return db.SaveChanges() > 0;
        }



        public static DBMessage[] GetMessages(int chatId, int offset, int count)
        {
            using var db = new TmdbContext();

            return db.Messages
                .Include(m => m.Action)
                .Where(m => m.DestinationId == chatId)
                .OrderByDescending(m => m.SendTime)
                .ThenByDescending(m => m.Id)
                .Skip(offset)
                .Take(count)
                .ToArray();
        }
        public static DBMessage[] GetLastMessages(int[] chatId)
        {
            using var db = new TmdbContext();

            return chatId.Select(id => db.Messages.Where(m => m.DestinationId == id && !m.IsSystem)
                                                  .AsEnumerable()
                                                  .MaxBy(m => m.Id))
                         .Where(m => m != null)
                         .ToArray();
        }
        public static DBMessage[] GetMessages(int chatId, int offset, int count, int lastMessageId)
        {
            using var db = new TmdbContext();

            return db.Messages.Include(m => m.Action)
                              .Where(m => m.DestinationId == chatId)
                              .OrderByDescending(m => m.SendTime)
                              .ThenByDescending(m => m.Id)
                              .Where(m => m.Id < lastMessageId)
                              .Skip(offset)
                              .Take(count)
                              .ToArray();
        }
        public static DBMessage[] GetMessages(int[] ids)
        {
            using var db = new TmdbContext();

            return db.Messages.Include(m => m.Action)
                              .OrderByDescending(m => m.SendTime)
                              .ThenByDescending(m => m.Id)
                              .Where(m => ids.Contains(m.Id))
                              .ToArray();
        }
        public static bool ReadAllInChat(int userId, int chatId)
        {
            using var db = new TmdbContext();
            //Чтение всех собщений в чате для юзера userId и отметка о прочитке собщений их авторам 
            var messsagesToMark =
                db.UnreadMessages.Include(um => um.Message)
                                 .Where(um => um.Message.DestinationId == chatId && userId != um.Message.AuthorId &&
                                       (um.UserId == userId || um.UserId == um.Message.AuthorId));

            db.UnreadMessages.RemoveRange(messsagesToMark);
            return db.SaveChanges(true) > 0;
        }
        public static bool MarkAsReaded(int userId, int[] ids)
        {
            using var db = new TmdbContext();

            var messsagesToMark = db.UnreadMessages.Include(um => um.Message)
                                   .Where(um => (um.UserId == userId || um.UserId == um.Message.AuthorId) &&
                                          ids.Contains(um.MessageId));

            db.UnreadMessages.RemoveRange(messsagesToMark);
            try
            {
                return db.SaveChanges(true) > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                return true;
            }
        }
        public static bool IsMessageReaded(int userId, int messageId)
        {
            using var db = new TmdbContext();
            return db.UnreadMessages.All(m => m.UserId != userId || m.MessageId != messageId);
        }
        public static bool[] IsMessageReaded(int userId, IEnumerable<int> messageIds)
        {
            return messageIds.Select(id => IsMessageReaded(userId, id))
                             .ToArray();
        }
    }
}
