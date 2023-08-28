using Microsoft.EntityFrameworkCore;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase
{
    internal class Chats
    {
        public static bool IsHaveAccess(int userId, int chatId)
        {
            using var db = new TmdbContext();

            return db.Chats.SingleOrDefault(c => c.MemberId == userId && c.ChatId == chatId) != null;
        }

        public static DBChat CreateChat(string name,params int[] usersId)
        {
            using var db = new TmdbContext();

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
            return chat;
        }

        public static void InviteToChat(int inviterId, int userId, int chatId)
        {
            using var db = new TmdbContext();

            if (!IsCanInvite(inviterId, userId, chatId))
                return;

            db.ChatInvites.Add(new DBChatInvite()
            {
                ChatId = chatId,
                InviterId = inviterId,
                UserId = userId,
            });
            db.SaveChanges();
        }

        public static bool IsCanInvite(int inviterId, int userId, int chatId)
        {
            using var db = new TmdbContext();

            bool isInviterInChat = db.Chats.Any(c => c.Id == chatId && c.MemberId == inviterId);
            bool isAlreadyInvited = db.ChatInvites.Any(i => i.UserId == userId && i.ChatId == chatId);
            bool isUserInChat = db.Chats.Any(c => c.Id == chatId && c.MemberId == userId);

            return isInviterInChat && !isAlreadyInvited && isUserInChat;
        }

        public static bool IsCanCreate(int userId,int[] memberIds)
        {
            using var db = new TmdbContext();
            db.Friends.Include(f=>f.UserIdOne==userId && )
        }

    }
}
