using TMServer.DataBase.Tables;

namespace TMServer.DataBase
{
    internal class Messages
    {
        public static void AddMessage(int authorId, string content, int destinationId)
        {
            var db = new TmdbContext();

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
            var db = new TmdbContext();

            return db.Messages.Where(m => m.DestinationId == chatId).Skip(offset).Take(count).ToArray();
        }

    }
}
