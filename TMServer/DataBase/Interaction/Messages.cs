using Microsoft.EntityFrameworkCore;
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
                SendTime = DateTime.UtcNow,
            };
            db.Messages.Add(message);
            db.SaveChanges(true);

            db.UnreadedMessages.Add(new DBUnreadedMessage()
            {
                MessageId = message.Id
            });
            db.SaveChanges();
            return message;
        }
        public static DBMessage[] GetMessages(int chatId, int offset, int count)
        {
            using var db = new TmdbContext();

            return db.Messages
                .Where(m => m.DestinationId == chatId)
                .OrderByDescending(m => m.SendTime)
                .ThenByDescending(m => m.Id)
                .Skip(offset)
                .Take(count)
                .ToArray();
        }
        public static DBMessage[] GetMessages(int chatId, int offset, int count, int lastMessageId)
        {
            using var db = new TmdbContext();

            return db.Messages
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

            return db.Messages
                .OrderByDescending(m => m.SendTime)
                .ThenByDescending(m => m.Id)
                .Where(m => ids.Contains(m.Id))
                .ToArray();
        }

        public static bool MarkAsReaded(int[] ids)
        {
            using var db = new TmdbContext();

            db.UnreadedMessages.Where(um => ids.Contains(um.MessageId)).ExecuteDelete();
            return db.SaveChanges() > 0;
        }
        public static bool IsMessageReaded(int messageId)
        {
            using var db = new TmdbContext();
            return !db.UnreadedMessages.Any(m => m.MessageId == messageId);
        }
        public static bool[] IsMessageReaded(IEnumerable<int> messageIds)
        {
            using var db = new TmdbContext();
            return messageIds.Select(id => !db.UnreadedMessages.Any(um => um.MessageId == id))
                             .ToArray();
        }
    }
}
