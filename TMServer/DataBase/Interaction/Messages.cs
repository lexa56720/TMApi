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
                .Where(m =>ids.Contains( m.Id) )
                .ToArray();
        }
    }
}
