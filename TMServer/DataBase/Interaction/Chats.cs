using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
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
                IsDialogue = false,
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
        public static void InviteToChat(int inviterId, int chatId, params int[] userIds)
        {
            using var db = new TmdbContext();

            foreach (var userId in userIds)
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
            var invite = db.ChatInvites.SingleOrDefault(i => i.Id == inviteId);
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
            db.SaveChanges(true);
        }
        public static DBChatInvite? RemoveInvite(int inviteId)
        {
            using var db = new TmdbContext();
            var invite = db.ChatInvites.SingleOrDefault(i => i.Id == inviteId);
            if (invite != null)
                db.ChatInvites.Remove(invite);
            db.SaveChanges();
            return invite;
        }
        public static void AddUserToChat(int userId, int chatId)
        {
            using var db = new TmdbContext();

            var user = db.Users.SingleOrDefault(u => u.Id == userId);
            var chat = db.Chats.SingleOrDefault(c => c.Id == chatId);
            if (user != null && chat != null)
                chat.Members.Add(user);
            db.SaveChanges(true);
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
