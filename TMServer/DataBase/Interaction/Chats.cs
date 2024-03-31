﻿using ApiTypes.Communication.Messages;
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
        public DBChat CreateChat(string name, params int[] usersId)
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
            Messages.AddCreateMessage(chat, usersId[0], db);
            db.SaveChanges(true);

            return chat;
        }
        public void InviteToChat(int inviterId, int chatId, params int[] userIds)
        {
            using var db = new TmdbContext();

            foreach (var userId in userIds)
                db.ChatInvites.Add(new DBChatInvite()
                {
                    ChatId = chatId,
                    InviterId = inviterId,
                    ToUserId = userId,
                });

            Messages.AddInviteMessages(chatId, inviterId, userIds, db);
            db.SaveChanges(true);
        }

        public DBChat[] GetChat(int[] chatIds)
        {
            using var db = new TmdbContext();

            var chats = db.Chats.Include(c => c.Admin)
                                .Include(c => c.Members)
                                .Where(c => chatIds.Contains(c.Id))
                                .ToArray();
            return chats;
        }
        public int[] GetUnreadCount(int userId, int[] chatIds)
        {
            using var db = new TmdbContext();

            return chatIds.Select(id => db.UnreadMessages.Include(um => um.Message)
                                      .ThenInclude(m => m.Destination)
                                      .Where(um => um.UserId == userId && id == um.Message.Destination.Id)
                                      .Count()).ToArray();
        }

        public DBChat[] GetAllChats(int userId)
        {
            using var db = new TmdbContext();

            var chats = db.Chats.Include(c => c.Admin)
                                .Include(c => c.Members)
                                .Where(c => c.Members.Any(m => m.Id == userId))
                                .ToArray();
            return chats;
        }

        public DBChatInvite[] GetInvite(int[] inviteIds, int userId)
        {
            using var db = new TmdbContext();
            return db.ChatInvites
                  .Where(i => (i.ToUserId == userId || i.InviterId == userId) && inviteIds.Contains(i.Id))
                  .ToArray();
        }
        public void InviteResponse(int inviteId, int userId, bool isAccepted)
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
        public DBChatInvite? RemoveInvite(int inviteId)
        {
            using var db = new TmdbContext();
            var invite = db.ChatInvites.SingleOrDefault(i => i.Id == inviteId);
            if (invite != null)
                db.ChatInvites.Remove(invite);
            db.SaveChanges();
            return invite;
        }
        public void AddUserToChat(int userId, int chatId)
        {
            using var db = new TmdbContext();

            var user = db.Users.SingleOrDefault(u => u.Id == userId);
            var chat = db.Chats.SingleOrDefault(c => c.Id == chatId);
            if (user != null && chat != null)
                chat.Members.Add(user);

            Messages.AddEnterMessage(chatId, userId, db);
            db.SaveChanges(true);
        }

        public bool LeaveChat(int userId, int chatId)
        {
            using var db = new TmdbContext();

            var chat = db.Chats.Include(c => c.Members)
                               .Where(i => i.Id == chatId)
                               .Single();

            if (chat.IsDialogue)
                return false;

            chat.Members.Remove(chat.Members.Single(m => m.Id == userId));

            if (chat.AdminId == userId)
            {
                var newAdmin = chat.Members.FirstOrDefault();
                if (newAdmin != null)
                    chat.AdminId = newAdmin.Id;
                else
                    RemoveChat(chat.Id, db);
            }

            Messages.AddLeaveMessage(chatId, userId, db);
            return db.SaveChanges(true) > 0;
        }
        public int[] GetAllChatInvites(int userId)
        {
            using var db = new TmdbContext();
            return db.ChatInvites.Where(i => i.ToUserId == userId)
                                 .Select(i => i.Id)
                                 .ToArray();
        }

        public void RemoveChat(int chatId, TmdbContext? db)
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
                db.SaveChanges();
                db.Dispose();
            }
        }

        public bool Rename(int chatId, string newName)
        {
            using var db = new TmdbContext();
            var chat = db.Chats.SingleOrDefault(c => c.Id == chatId);
            if (chat == null)
                return false;

            chat.Name = newName;
            return db.SaveChanges(true) > 0;
        }

        public bool Kick(int chatId, int userId, int kickId)
        {
            using var db = new TmdbContext();
            var chat = db.Chats.Include(c => c.Members)
                               .SingleOrDefault(c => c.Id == chatId);
            if (chat == null)
                return false;

            var member = chat.Members.SingleOrDefault(m => m.Id == kickId);
            if (member == null)
                return false;

            chat.Members.Remove(member);
            Messages.AddKickMessage(chat.Id, userId, kickId, db);
            return db.SaveChanges(true) > 0;
        }

        public DBChat? SetCover(int userId, int chatId,int imageId, out int prevSetId)
        {
            using var db = new TmdbContext();

            var chat = db.Chats.SingleOrDefault(u => u.Id == chatId);
            if (chat == null)
            {
                prevSetId = -1;
                return null;
            }
            prevSetId = chat.CoverImageId;
            chat.CoverImageId = imageId;
            db.SaveChanges(true);
            return chat;
        }
    }
}
