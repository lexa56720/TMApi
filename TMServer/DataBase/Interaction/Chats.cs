using Microsoft.EntityFrameworkCore;
using System.Linq;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase.Interaction
{
    internal static class Chats
    {
        public static DBChat CreateChat(string name, params int[] usersId)
        {
            using var db = new TmdbContext();

            var chat = new DBChat()
            {
                AdminId = usersId[0],
                Name = name,
            };
            db.Chats.Add(chat);
            for (int i = 1; i < usersId.Length; i++)
            {
                var user = db.Users.Find(usersId[i]);
                if (user != null)
                    chat.Members.Add(user);
            }
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
                ToUserId = userId,
            });
            db.SaveChanges();
        }

        public static DBChat? GetChat(int chatId)
        {
            using var db = new TmdbContext();

            var chat = db.Chats.Include(c => c.Members).SingleOrDefault(c => c.Id == chatId);
            return chat;
        }
        public static DBChat[] GetChat(int[] chatIds)
        {
            using var db = new TmdbContext();

            var chats = db.Chats.Include(c => c.Members).Where(c => chatIds.Contains(c.Id)).ToArray();
            return chats;
        }

        public static DBChatInvite? GetInvite(int inviteId, int userId)
        {
            using var db = new TmdbContext();

            return db.ChatInvites
                .SingleOrDefault(i => (i.ToUserId == userId || i.InviterId == userId) && i.Id == inviteId);
        }

        public static DBChatInvite[] GetInvite(int[] inviteIds, int userId)
        {
            using var db = new TmdbContext();
            return db.ChatInvites
                  .Where(i => (i.ToUserId == userId || i.InviterId == userId) && inviteIds.Contains(i.Id))
                  .ToArray();

        }
        public static void InviteResponse(int inviteId, int userId, bool isAccepted)
        {
            using var db = new TmdbContext();
            var invite = db.ChatInvites.Find(inviteId);
            if (invite == null)
                return;

            if (isAccepted)
            {
                var user = db.Users.Find(userId);
                var chat = db.Chats.Find(invite.ChatId);
                if (user != null && chat != null && invite != null)
                    chat.Members.Add(user);
            }
            db.ChatInvites.Remove(invite);
            db.SaveChanges();
        }

        public static int[] GetAllForUser(int userId)
        {
            using var db = new TmdbContext();
            return db.ChatInvites.Where(i => i.ToUserId == userId)
                                 .Select(i => i.Id).ToArray();
        }
        public static bool IsHaveAccess(int chatId, int userId)
        {
            using var db = new TmdbContext();

            return IsMemberOfChat(userId, chatId) || IsInvited(userId, chatId);
        }
        public static bool IsInvited(int userId, int chatId)
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
        public static bool IsCanInvite(int inviterId, int userId, int chatId)
        {
            using var db = new TmdbContext();

            bool isFriends = db.Friends
                .SingleOrDefault(f => f.UserIdOne == inviterId && f.UserIdTwo == userId
                                   || f.UserIdOne == userId && f.UserIdTwo == inviterId) != null;

            bool isInviterInChat = db.Chats.Include(c => c.Members).Any(c => c.Id == chatId && c.Members.Any(m => m.Id == inviterId));
            bool isAlreadyInvited = IsInvited(userId, chatId);
            bool isUserInChat = db.Chats.Include(c => c.Members).Any(c => c.Id == chatId && c.Members.Any(m => m.Id == userId));

            return isInviterInChat && !isAlreadyInvited && isUserInChat && isFriends;
        }
        public static bool IsCanCreate(int userId, int[] memberIds)
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
