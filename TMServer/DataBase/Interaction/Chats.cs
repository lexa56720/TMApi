using Microsoft.EntityFrameworkCore;
using System.Linq;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase.Interaction
{
    internal static class Chats
    {
        public static DBChat CreateChat(string name, bool isDialogue, params int[] usersId)
        {
            using var db = new TmdbContext();
            var chat = new DBChat()
            {
                AdminId = usersId[0],
                IsDialogue = isDialogue,
                Name = name,
            };
            db.Chats.Add(chat);
            for (int i = 0; i < usersId.Length; i++)
            {
                var user = db.Users.Find(usersId[i]);
                if (user != null)
                    chat.Members.Add(user);
            }
            db.SaveChanges(true);
            return chat;
        }
        public static void InviteToChat(int inviterId, int userId, int chatId)
        {
            using var db = new TmdbContext();

            if (!Security.IsCanInviteToChat(inviterId, userId, chatId))
                return;

            db.ChatInvites.Add(new DBChatInvite()
            {
                ChatId = chatId,
                InviterId = inviterId,
                ToUserId = userId,
            });
            db.SaveChanges(true);
        }

        public static DBChat[] GetChat(int[] chatIds)
        {
            using var db = new TmdbContext();

            var chats = db.Chats.Include(c => c.Admin)
                                .Include(c => c.Members)
                                .Where(c => chatIds.Contains(c.Id))
                                .ToArray();
            return chats;
        }
        public static int[] GetUnreadCount(int userId, int[] chatIds)
        {
            using var db = new TmdbContext();

            return chatIds.Select(id => db.UnreadMessages.Include(um => um.Message)
                                      .ThenInclude(m => m.Destination)
                                      .Where(um => um.UserId == userId && id == um.Message.Destination.Id)
                                      .Count()).ToArray();
        }

        public static DBChat[] GetAllChats(int userId)
        {
            using var db = new TmdbContext();

            var chats = db.Chats.Include(c => c.Admin)
                                .Include(c => c.Members)
                                .Where(c => c.Members.Any(m => m.Id == userId))
                                .ToArray();
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

        public static int[] GetAllChatInvites(int userId)
        {
            using var db = new TmdbContext();
            return db.ChatInvites.Where(i => i.ToUserId == userId)
                                 .Select(i => i.Id)
                                 .ToArray();
        }
    }
}
