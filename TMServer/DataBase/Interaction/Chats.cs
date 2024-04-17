using ApiTypes.Communication.Messages;
using CSDTP.Requests;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Linq;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase.Interaction
{
    public class Chats
    {
        private readonly Messages Messages;

        public Chats(Messages messages)
        {
            Messages = messages;
        }
        public async Task<DBChat> CreateChat(string name, params int[] usersId)
        {
            using var db = new TmdbContext();
            var chat = new DBChat()
            {
                AdminId = usersId[0],
                IsDialogue = false,
                Name = name,
            };
            await db.Chats.AddAsync(chat);
            for (int i = 0; i < usersId.Length; i++)
            {
                var user = await db.Users.SingleOrDefaultAsync(u => u.Id == usersId[i]);
                if (user != null)
                    chat.Members.Add(user);
            }

            await Messages.AddCreateMessage(chat, usersId[0], db);
            await Messages.AddInviteMessages(chat, usersId[0], chat.Members.Skip(1).Select(i => i.Id), db);
            await db.SaveChangesAsync(true);
            return chat;
        }
        public async Task InviteToChat(int inviterId, int chatId, params int[] userIds)
        {
            using var db = new TmdbContext();

            foreach (var userId in userIds)
                await db.ChatInvites.AddAsync(new DBChatInvite()
                {
                    ChatId = chatId,
                    InviterId = inviterId,
                    ToUserId = userId,
                });

            await Messages.AddInviteMessages(chatId, inviterId, userIds, db);
            db.SaveChanges(true);
        }

        public async Task<DBChat[]?> GetChat(int[] chatIds)
        {
            using var db = new TmdbContext();

            var chats = await db.Chats.Include(c => c.Admin)
                                      .Include(c => c.Members)
                                      .Where(c => chatIds.Contains(c.Id))
                                      .ToArrayAsync();
            return chats;
        }
        public async Task<int[]> GetUnreadCount(int userId, int[] chatIds)
        {
            using var db = new TmdbContext();

            var result = new int[chatIds.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = await db.UnreadMessages
                    .Include(um => um.Message)
                    .ThenInclude(m => m.Destination)
                    .Where(um => um.UserId == userId && chatIds[i] == um.Message.Destination.Id)
                    .CountAsync();
            }

            return result;
        }

        public async Task<DBChat[]> GetAllChats(int userId)
        {
            using var db = new TmdbContext();

            var chats = await db.Chats.Include(c => c.Admin)
                                      .Include(c => c.Members)
                                      .Where(c => c.Members.Any(m => m.Id == userId))
                                      .ToArrayAsync();
            return chats;
        }

        public async Task<DBChatInvite[]> GetInvite(int[] inviteIds, int userId)
        {
            using var db = new TmdbContext();
            return await db.ChatInvites.Where(i => (i.ToUserId == userId || i.InviterId == userId)
                                              && inviteIds.Contains(i.Id))
                                       .ToArrayAsync();
        }
        public async Task InviteResponse(int inviteId, int userId, bool isAccepted)
        {
            using var db = new TmdbContext();
            var invite = await db.ChatInvites.SingleOrDefaultAsync(i => i.Id == inviteId);
            if (invite == null)
                return;

            if (isAccepted)
            {
                var user = await db.Users.FindAsync(userId);
                var chat = await db.Chats.FindAsync(invite.ChatId);
                if (user != null && chat != null && invite != null)
                    chat.Members.Add(user);
            }
            db.ChatInvites.Remove(invite);
            await db.SaveChangesAsync(true);
        }
        public async Task<DBChatInvite?> RemoveInvite(int inviteId)
        {
            using var db = new TmdbContext();
            var invite = await db.ChatInvites.SingleOrDefaultAsync(i => i.Id == inviteId);
            if (invite != null)
                db.ChatInvites.Remove(invite);
            await db.SaveChangesAsync();
            return invite;
        }
        public async Task AddUserToChat(int userId, int chatId)
        {
            using var db = new TmdbContext();

            var user = await db.Users.SingleOrDefaultAsync(u => u.Id == userId);
            var chat = await db.Chats.SingleOrDefaultAsync(c => c.Id == chatId);
            if (user != null && chat != null)
            {
                chat.Members.Add(user);
                await Messages.AddEnterMessage(chatId, userId, db);
            }

            await db.SaveChangesAsync(true);
        }

        public async Task<bool> LeaveChat(int userId, int chatId)
        {
            using var db = new TmdbContext();

            var chat = await db.Chats.Include(c => c.Members)
                                     .Where(i => i.Id == chatId)
                                     .SingleAsync();

            if (chat.IsDialogue)
                return false;

            chat.Members.Remove(chat.Members.Single(m => m.Id == userId));

            if (chat.AdminId == userId)
            {
                var newAdmin = chat.Members.FirstOrDefault();
                if (newAdmin != null)
                    chat.AdminId = newAdmin.Id;
                else
                    await RemoveChat(chat.Id, db);
            }

            await Messages.AddLeaveMessage(chatId, userId, db);
            return await db.SaveChangesAsync(true) > 0;
        }
        public async Task<int[]> GetAllChatInvites(int userId)
        {
            using var db = new TmdbContext();
            return await db.ChatInvites.Where(i => i.ToUserId == userId)
                                 .Select(i => i.Id)
                                 .ToArrayAsync();
        }

        public async Task RemoveChat(int chatId, TmdbContext? db)
        {
            bool needToDispose = false;
            if (db == null)
            {
                db = new TmdbContext();
                needToDispose = true;
            }

            db.Messages.Where(m => m.DestinationId == chatId).ExecuteDelete();
            db.ChatInvites.Where(i => i.ChatId == chatId).ExecuteDelete();
            db.Chats.Where(c => c.Id == chatId).ExecuteDelete();

            if (needToDispose)
            {
                await db.SaveChangesAsync();
                await db.DisposeAsync();
            }
        }

        public async Task<bool> Rename(int chatId, int userId, string newName)
        {
            using var db = new TmdbContext();
            var chat = await db.Chats.SingleOrDefaultAsync(c => c.Id == chatId);
            if (chat == null)
                return false;

            chat.Name = newName;
            await Messages.AddRenameMessage(chat.Id, userId, newName, db);
            return await db.SaveChangesAsync(true) > 0;
        }

        public async Task<bool> Kick(int chatId, int userId, int kickId)
        {
            using var db = new TmdbContext();
            var chat = await db.Chats.Include(c => c.Members)
                                     .SingleOrDefaultAsync(c => c.Id == chatId);
            if (chat == null)
                return false;

            var member = chat.Members.SingleOrDefault(m => m.Id == kickId);
            if (member == null)
                return false;

            chat.Members.Remove(member);
            await Messages.AddKickMessage(chat.Id, userId, kickId, db);
            return await db.SaveChangesAsync(true) > 0;
        }

        public async Task<(DBChat?, int prevSetId)> SetCover(int userId, int chatId, int imageId)
        {
            using var db = new TmdbContext();

            var chat = await db.Chats.SingleOrDefaultAsync(u => u.Id == chatId);
            if (chat == null)
                return (null, -1);

            var prevSetId = chat.CoverImageId;
            chat.CoverImageId = imageId;
            await Messages.AddUpdateCoverMessage(chatId, userId, db);
            await db.SaveChangesAsync(true);
            return (chat, prevSetId);
        }
    }
}
