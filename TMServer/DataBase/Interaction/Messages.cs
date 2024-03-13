using ApiTypes.Communication.Messages;
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

            return message;
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

            return chatId.Select(id => db.Messages.Where(m => m.DestinationId == id).AsEnumerable().MaxBy(m => m.Id))
                         .Where(m=>m!=null)
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
        public static bool ReadAllInChat(int userId, int chatId)
        {
            using var db = new TmdbContext();

            var messsagesToMark =
                db.UnreadMessages.Include(um => um.Message)
                                   .Where(um => um.Message.DestinationId == chatId &&
                                          (um.UserId == userId && um.UserId != um.Message.AuthorId));

            db.UnreadMessages.RemoveRange(messsagesToMark);
            return db.SaveChanges(true) > 0;
        }
        public static bool MarkAsReaded(int userId, int[] ids)
        {
            using var db = new TmdbContext();

            var messsagesToMark =
                db.UnreadMessages.Include(um => um.Message)
                                   .Where(um => (um.UserId == userId || um.UserId == um.Message.AuthorId) &&
                                          ids.Contains(um.MessageId));

            db.UnreadMessages.RemoveRange(messsagesToMark);
            return db.SaveChanges(true) > 0;
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
