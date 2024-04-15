using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Messages;
using ApiTypes.Shared;
using CSDTP;
using CSDTP.Requests;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase.Interaction
{
    public class Security
    {
        private readonly int MaxFileSizeMb;
        private readonly int MaxFilesInMessage;

        public Security(int maxFileSizeMB, int maxFilesInMessage)
        {
            MaxFileSizeMb = maxFileSizeMB;
            MaxFilesInMessage = maxFilesInMessage;
        }

        public async Task<bool> IsTokenCorrect(string token, int userId)
        {
            using var db = new TmdbContext();
            return await db.Tokens.AnyAsync(t => t.UserId == userId && token.Equals(t.AccessToken) && DateTime.UtcNow < t.Expiration);
        }

        public async Task<bool> IsHaveAccessToChat(int chatId, int userId)
        {
            return await IsMemberOfChat(userId, chatId) || await IsInvitedToChat(userId, chatId);
        }
        public async Task<bool> IsHaveAccessToChat(int[] chatIds, int userId)
        {
            var tasks = new Task<bool>[chatIds.Length];
            for (int i = 0; i < chatIds.Length; i++)
                tasks[i] = IsHaveAccessToChat(chatIds[i], userId);
            var result = await Task.WhenAll(tasks);
            return result.All(r => r == true);
        }

        public async Task<bool> IsInvitedToChat(int userId, int chatId)
        {
            using var db = new TmdbContext();

            return await db.ChatInvites.AnyAsync(i => i.ToUserId == userId && i.ChatId == chatId);
        }
        public async Task<bool> IsMemberOfChat(int userId, int chatId)
        {
            using var db = new TmdbContext();

            return await db.Chats.Include(c => c.Members)
                                 .AnyAsync(c => c.Id == chatId && c.Members.Any(m => m.Id == userId));
        }
        public async Task<bool> IsAdminOfChat(int userId, int chatId)
        {
            using var db = new TmdbContext();
            return await db.Chats.AnyAsync(c => c.Id == chatId && c.AdminId == userId);
        }

        public async Task<bool> IsCanInviteToChat(int inviterId, int[] userIds, int chatId)
        {
            bool result = true;
            for (var i = 0; i < userIds.Length; i++)
                if (!await IsCanInviteToChat(inviterId, userIds[i], chatId))
                {
                    result = false;
                    break;
                }
            return result;
        }
        public async Task<bool> IsCanInviteToChat(int inviterId, int userId, int chatId)
        {
            using var db = new TmdbContext();

            bool isFriends = await db.Friends.SingleOrDefaultAsync(f => f.SenderId == inviterId && f.DestId == userId ||
                                                        f.SenderId == userId && f.DestId == inviterId) != null;

            bool isInviterInChat = await db.Chats.Include(c => c.Members)
                                           .AnyAsync(c => c.Id == chatId && c.Members.Any(m => m.Id == inviterId));

            var chat = await db.Chats.Include(c => c.Members).Where(c => c.Id == chatId).SingleAsync();
            bool isUserInChat = chat.Members.Any(m => m.Id == userId);

            bool isAlreadyInvited = await IsInvitedToChat(userId, chatId);

            return !chat.IsDialogue && inviterId != userId && isInviterInChat &&
                   !isAlreadyInvited && !isUserInChat && isFriends;
        }

        public async Task<bool> IsInviteExist(int inviteId, int userId)
        {
            using var db = new TmdbContext();

            return await db.ChatInvites.AnyAsync(i => (i.ToUserId == userId || i.InviterId == userId) && i.Id == inviteId);
        }
        public async Task<bool> IsCanCreateChat(int userId, int[] memberIds)
        {
            using var db = new TmdbContext();
            var user = await db.Users.Include(u => u.FriendsTwo).ThenInclude(f => f.Sender)
                               .Include(u => u.FriendsOne).ThenInclude(f => f.Receiver)
                               .SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return false;

            var frinendsId = user.GetFriends().Select(f => f.Id);
            if (user == null || memberIds.Distinct().Count() != memberIds.Length ||
                memberIds.Contains(userId) || !memberIds.All(f => frinendsId.Contains(f)))
                return false;

            return true;
        }

        public async Task<bool> IsFriendshipPossible(int fromId, int toId)
        {
            using var db = new TmdbContext();
            return toId != fromId && !await IsFriends(fromId, toId) &&
                   !db.FriendRequests.Any(r => r.SenderId == fromId && r.ReceiverId == toId);
        }
        public async Task<bool> IsFriends(int idOne, int idTwo)
        {
            using var db = new TmdbContext();
            return await db.Friends.AnyAsync(f => (f.SenderId == idOne && f.DestId == idTwo)
                                         || (f.SenderId == idTwo && f.DestId == idOne));
        }
        public async Task<bool> IsHaveAccessToRequest(int userId, params int[] requestIds)
        {
            using var db = new TmdbContext();

            return await db.FriendRequests.Where(r => requestIds.Contains(r.Id))
                                          .AllAsync(r => r.SenderId == userId || r.ReceiverId == userId);
        }
        public async Task<bool> IsExistOppositeRequest(int fromId, int toId)
        {
            using var db = new TmdbContext();
            return await db.FriendRequests.AnyAsync(r => r.ReceiverId == fromId && r.SenderId == toId);
        }
        public async Task<bool> IsExistFriendRequest(int requestId, int userId)
        {
            using var db = new TmdbContext();
            var request = await db.FriendRequests.SingleOrDefaultAsync(r => r.Id == requestId);
            return request != null && (request.SenderId == userId || request.ReceiverId == userId);
        }

        public async Task<bool> IsHaveAccessToMessages(int userId, params int[] messagesIds)
        {
            using var db = new TmdbContext();

            var chats = await db.Messages.Where(m => messagesIds.Contains(m.Id))
                                   .Include(m => m.Destination)
                                   .ThenInclude(c => c.Members)
                                   .Select(m => m.Destination)
                                   .ToArrayAsync();

            return chats.DistinctBy(c => c.Id)
                        .All(c => c.Members.Any(m => m.Id == userId));
        }
        public async Task<bool> IsCanMarkAsReaded(int userId, params int[] messagesIds)
        {
            using var db = new TmdbContext();
            if (!await db.Messages.Where(m => messagesIds.Contains(m.Id)).AllAsync(m => m.AuthorId != userId))
                return false;

            var chats = await db.Messages.Where(m => messagesIds.Contains(m.Id))
                                   .Include(m => m.Destination)
                                   .ThenInclude(c => c.Members)
                                   .Select(m => m.Destination)
                                   .ToArrayAsync();
   
            return chats.DistinctBy(c => c.Id)
                        .All(c => c.Members.Any(m => m.Id == userId));
        }

        public async Task<bool> IsCryptIdCorrect(int userId, int cryptId)
        {
            using var db = new TmdbContext();

            var result = await db.AesCrypts.SingleOrDefaultAsync(c => c.UserId == userId && c.Id == cryptId);
            return result != null;
        }


        public bool IsValidProfileImage(Image image)
        {
            if (image.Width >= 64 && image.Height >= 64 &&
                image.Width <= 1024 && image.Height <= 1024)
                return true;

            image.Dispose();
            return false;
        }

        public bool IsValidImage(Image image)
        {
            if (image.Width >= 64 && image.Height >= 64 && 
                image.Width <= 2048 && image.Height <= 2048)
                return true;

            image.Dispose();
            return false;
        }

        public async Task<bool> IsMessageWithFilesLegal(int userId, MessageWithFilesSendRequest request)
        {
            return request.Files.Length != 0
                   && DataConstraints.IsMessageWithFilesLegal(request.Text)
                   && await IsMemberOfChat(userId, request.DestinationId)
                   && IsAttachmentsLegal(request.Files);
        }
        public bool IsAttachmentsLegal(SerializableFile[] files)
        {
            return files.All(f => f.Data.Length < (MaxFileSizeMb * Math.Pow(10, 6))) && files.Length <= MaxFilesInMessage;
        }
    }
}
