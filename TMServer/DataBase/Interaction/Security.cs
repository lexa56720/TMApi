using ApiTypes.Communication.BaseTypes;
using CSDTP;
using CSDTP.Requests;
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
            return IsMemberOfChat(userId, chatId) || IsInvitedToChat(userId, chatId);
        }
        public static bool IsHaveAccessToChat(int[] chatIds, int userId)
        {
            return chatIds.All(c => IsHaveAccessToChat(c, userId));
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

        public static bool IsCanInviteToChat(int inviterId, int[] userIds, int chatId)
        {
            bool result = true;
            for (var i = 0; i < userIds.Length; i++)
                if (!IsCanInviteToChat(inviterId, userIds[i], chatId))
                {
                    result = false;
                    break;
                }
            return result;
        }
        public static bool IsCanInviteToChat(int inviterId, int userId, int chatId)
        {
            using var db = new TmdbContext();

            bool isFriends = db.Friends.SingleOrDefault(f => f.SenderId == inviterId && f.DestId == userId ||
                                                        f.SenderId == userId && f.DestId == inviterId) != null;

            bool isInviterInChat = db.Chats.Include(c => c.Members)
                                           .Any(c => c.Id == chatId && c.Members.Any(m => m.Id == inviterId));

            var chat = db.Chats.Include(c => c.Members).Where(c => c.Id == chatId).Single();
            bool isUserInChat = chat.Members.Any(m => m.Id == userId);

            bool isAlreadyInvited = IsInvitedToChat(userId, chatId);

            return !chat.IsDialogue && inviterId != userId && isInviterInChat &&
                   !isAlreadyInvited && !isUserInChat && isFriends;
        }

        public static bool IsHaveAccessToInvite(int inviteId, int userId)
        {
            using var db = new TmdbContext();

            return db.ChatInvites.Any(i => (i.ToUserId == userId || i.InviterId == userId) && i.Id == inviteId);
        }
        public static bool IsCanCreateChat(int userId, int[] memberIds)
        {
            using var db = new TmdbContext();
            var user = db.Users.Include(u => u.FriendsTwo).ThenInclude(f => f.Sender)
                               .Include(u => u.FriendsOne).ThenInclude(f => f.Receiver)
                               .SingleOrDefault(u => u.Id == userId);

            var frinendsId = user?.GetFriends().Select(f=>f.Id);
            if (user == null || memberIds.Distinct().Count() != memberIds.Length ||
                memberIds.Contains(userId) || !memberIds.All(f => frinendsId.Contains(f)))
                return false;

            return true;
        }

        public static bool IsFriendshipPossible(int fromId, int toId)
        {
            using var db = new TmdbContext();
            return toId != fromId && !IsAlreadyFriends(fromId, toId) &&
                   !db.FriendRequests.Any(r => r.SenderId == fromId && r.ReceiverId == toId);
        }
        public static bool IsAlreadyFriends(int idOne, int idTwo)
        {
            using var db = new TmdbContext();
            return db.Friends.Any(f => (f.SenderId == idOne && f.DestId == idTwo)
                                    || (f.SenderId == idTwo && f.DestId == idOne));
        }
        public static bool IsHaveAccessToRequest(int userId, params int[] requestIds)
        {
            using var db = new TmdbContext();

            return db.FriendRequests.Where(r => requestIds.Contains(r.Id))
                                    .All(r => r.SenderId == userId || r.ReceiverId == userId);
        }
        public static bool IsExistOppositeRequest(int fromId, int toId)
        {
            using var db = new TmdbContext();
            return db.FriendRequests.Any(r => r.ReceiverId == fromId && r.SenderId == toId);
        }
        public static bool IsExistFriendRequest(int requestId, int userId)
        {
            using var db = new TmdbContext();
            var request = db.FriendRequests.SingleOrDefault(r => r.Id == requestId);
            return request != null && (request.SenderId == userId || request.ReceiverId == userId);
        }

        public static bool IsHaveAccessToMessages(int userId, params int[] messagesIds)
        {
            using var db = new TmdbContext();

            var chats = db.Messages.Where(m => messagesIds.Contains(m.Id))
                                   .Include(m => m.Destination)
                                   .ThenInclude(c => c.Members)
                                   .Select(m => m.Destination)
                                   .AsEnumerable()
                                   .DistinctBy(c => c.Id);
            return chats.All(c => c.Members.Any(m => m.Id == userId));
        }
        public static bool IsCanMarkAsReaded(int userId, params int[] messagesIds)
        {
            using var db = new TmdbContext();
            if (!db.Messages.Where(m => messagesIds.Contains(m.Id)).All(m => m.AuthorId != userId))
                return false;

            var chats = db.Messages.Where(m => messagesIds.Contains(m.Id))
                                   .Include(m => m.Destination)
                                   .ThenInclude(c => c.Members)
                                   .Select(m => m.Destination)
                                   .AsEnumerable()
                                   .DistinctBy(c => c.Id);
            return chats.All(c => c.Members.Any(m => m.Id == userId));
        }
    }
}
