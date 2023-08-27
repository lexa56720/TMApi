namespace TMServer.DataBase
{
    internal class Chats
    {
        public static bool IsHaveAccess(int userId, int chatId)
        {
            var db = new TmdbContext();

            return db.Chats.SingleOrDefault(c => c.MemberId == userId && c.ChatId == chatId) != null;
        }
    }
}
