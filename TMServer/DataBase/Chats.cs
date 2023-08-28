using TMServer.DataBase.Tables;

namespace TMServer.DataBase
{
    internal class Chats
    {
        public static bool IsHaveAccess(int userId, int chatId)
        {
            var db = new TmdbContext();

            return db.Chats.SingleOrDefault(c => c.MemberId == userId && c.ChatId == chatId) != null;
        }

        public static void CreateChat(params int[] usersId)
        {
            var db = new TmdbContext();

            var chat = new DBChat()
            {
                AdminId = usersId[0],
                MemberId = usersId[1],
            };

            db.Chats.Add(chat);

            var members = new DBChat[usersId.Length - 2];
            for (int i = 0; i < members.Length; i++)
            {
                members[i] = new DBChat()
                {
                    AdminId = usersId[0],
                    ChatId = chat.Id,
                    MemberId = usersId[i + 2],
                };
            }
            db.Chats.AddRange(members);
            db.SaveChanges();
        }
    }
}
