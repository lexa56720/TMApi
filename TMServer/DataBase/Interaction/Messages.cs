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
            db.UnreadedMessages.Add(new DBUnreadedMessage()
            {
                Message = message
            });
            db.SaveChanges(true);
            return message;
        }

        public static bool ReadAllInChat(int userId,int chatId)
        {
            using var db = new TmdbContext();

           var readed= db.UnreadedMessages.Include(um => um.Message)
                                          .Where(um => um.Message.DestinationId == chatId && um.Message.AuthorId != userId);

            db.UnreadedMessages.RemoveRange(readed);
            db.SaveChanges(true);
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

            db.UnreadedMessages.RemoveRange(db.UnreadedMessages.Where(um => ids.Contains(um.MessageId)));
            return db.SaveChanges(true) > 0;
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
