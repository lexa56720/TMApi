using TMServer.DataBase.Tables;

namespace TMServer.DataBase.Interaction
{
    internal class Messages
    {
        public static void AddMessage(int authorId, string content, int destinationId)
        {
            using var db = new TmdbContext();

            db.Messages.Add(new DBMessage()
            {
                AuthorId = authorId,
                DestinationId = destinationId,
                Content = content,
                SendTime = DateTime.UtcNow,
            });

            db.SaveChanges();
        }

        public static DBMessage[] GetMessages(int chatId, int offset, int count)
        {
            using var db = new TmdbContext();

            return db.Messages
                .Where(m => m.DestinationId == chatId)
                .Skip(offset)
                .Take(count)
                .ToArray();
        }
        public static DBMessage[] GetMessages(int chatId, int offset, int count, int lastMessageId, DateTime lastMessageDate)
        {
           using var db = new TmdbContext();

            return db.Messages
                .Where(m => m.DestinationId == chatId)
                .OrderBy(m => m.SendTime)
                .ThenBy(m => m.Id)
                .Where(m => m.SendTime < lastMessageDate || (m.SendTime == lastMessageDate && m.Id < lastMessageId))
                .Skip(offset)
                .Take(count)
                .ToArray();
        }
    }
}
