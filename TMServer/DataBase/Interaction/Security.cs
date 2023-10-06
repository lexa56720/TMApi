using CSDTP;
using Microsoft.EntityFrameworkCore;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase.Interaction
{
    internal static class Security
    {
        public static bool IsTokenCorrect(string token, int userId)
        {
            using var db = new TmdbContext();
            return db.Tokens.Any(t => t.UserId == userId && token.Equals(t.AccessToken) && DateTime.UtcNow < t.Expiration);
        }

        public static bool IsHaveAccessToChat(int chatId, int userId)
        {
            using var db = new TmdbContext();

            return IsMemberOfChat(userId, chatId) || IsInvitedToChat(userId, chatId);
        }

        public static bool IsInvitedToChat(int userId, int chatId)
        {
            using var db = new TmdbContext();

            return db.ChatInvites.Any(i => i.ToUserId == userId && i.ChatId == chatId);
        }
        public static bool IsMemberOfChat(int userId, int chatId)
        {
            using var db = new TmdbContext();

            return db.Chats.Include(c => c.Members)
                .Any(c => c.Id == chatId && c.Members.Any(m => m.Id == userId));
        }

        public static bool IsCanInviteToChat(int inviterId, int userId, int chatId)
        {
            using var db = new TmdbContext();

            bool isFriends = db.Friends
                .SingleOrDefault(f => f.SenderId == inviterId && f.DestId == userId
                                   || f.SenderId == userId && f.DestId == inviterId) != null;

            bool isInviterInChat = db.Chats.Include(c => c.Members).Any(c => c.Id == chatId && c.Members.Any(m => m.Id == inviterId));
            bool isAlreadyInvited = IsInvitedToChat(userId, chatId);
            bool isUserInChat = db.Chats.Include(c => c.Members).Any(c => c.Id == chatId && c.Members.Any(m => m.Id == userId));

            return isInviterInChat && !isAlreadyInvited && isUserInChat && isFriends;
        }

        public static bool IsCanCreateChat(int userId, int[] memberIds)
        {
            using var db = new TmdbContext();
            var user = db.Users
                  .Include(u => u.FriendsOne)
                  .Include(u => u.FriendsTwo)
                  .SingleOrDefault(u => u.Id == userId);

            if (user == null)
                return false;

            for (int i = 0; i < memberIds.Length; i++)
                if (!user.Friends.Any(u => u.Id == memberIds[i]))
                    return false;

            return true;
        }

    }
}
