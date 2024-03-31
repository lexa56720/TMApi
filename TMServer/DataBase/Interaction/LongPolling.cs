using ApiTypes.Communication.LongPolling;
using Microsoft.EntityFrameworkCore;
using TMServer.DataBase.Tables.LongPolling;
using TMServer.ServerComponent.LongPolling;

namespace TMServer.DataBase.Interaction
{
    public class LongPolling
    {
        public DBNewMessageUpdate[] GetMessageUpdate(int userId)
        {
            using var db = new TmdbContext();
            var result = db.NewMessageUpdates
                           .Where(c => c.UserId == userId)
                           .ToArray();
            return result;
        }

        public DBMessageStatusUpdate[] GetMessagesWithUpdatedStatus(int userId)
        {
            using var db = new TmdbContext();
            var result = db.MessageStatusUpdates
                           .Where(c => c.UserId == userId)
                           .ToArray();
            return result;
        }
        public DBFriendRequestUpdate[] GetFriendRequestUpdates(int userId)
        {
            using var db = new TmdbContext();
            var result = db.FriendRequestUpdates
                           .Where(r => r.UserId == userId)
                           .ToArray();
            return result;
        }
        public DBFriendListUpdate[] GetFriendListUpdates(int userId)
        {
            using var db = new TmdbContext();
            var result = db.FriendListUpdates
                           .Where(fl => fl.UserId == userId)
                           .ToArray();

            return result;
        }

        public DBChatListUpdate[] GetChatListUpdates(int userId)
        {
            using var db = new TmdbContext();
            var result = db.ChatListUpdates
                           .Where(cu => cu.UserId == userId)
                           .ToArray();
            return result;
        }

        public DBUserProfileUpdate[] GetRelatedUsersUpdates(int userId)
        {
            using var db = new TmdbContext();
            var result = db.UserProfileUpdates
                           .Where(u => u.UserId == userId)
                           .ToArray();
            return result;
        }

        public DBChatInviteUpdate[] GetChatInvites(int userId)
        {
            using var db = new TmdbContext();
            var result = db.ChatInviteUpdates
                           .Where(c => c.UserId == userId)
                           .ToArray();
            return result;
        }

        public DBChatUpdate[] GetChatUpdates(int userId)
        {
            using var db = new TmdbContext();
            var result = db.ChatUpdates
                           .Where(c => c.UserId == userId)
                           .ToArray();
            return result;
        }


        public void ClearAllUpdates(int userId)
        {
            using var db = new TmdbContext();
            db.NewMessageUpdates.Where(u => u.UserId == userId).ExecuteDelete();
            db.MessageStatusUpdates.Where(u => u.UserId == userId).ExecuteDelete();
            db.FriendRequestUpdates.Where(fr => fr.UserId == userId).ExecuteDelete();
            db.ChatListUpdates.Where(c => c.UserId == userId).ExecuteDelete();
            db.ChatInviteUpdates.Where(c => c.UserId == userId).ExecuteDelete();
            db.ChatUpdates.Where(c => c.UserId == userId).ExecuteDelete();
            db.UserProfileUpdates.Where(p => p.UserId == userId).ExecuteDelete();
            db.FriendListUpdates.Where(fl => fl.UserId == userId).ExecuteDelete();
            db.SaveChanges();
        }
        public void ClearUpdatesByIds(LongPollResponseInfo info)
        {
            using var db = new TmdbContext();
            ExecuteDeleteByIds(db.NewMessageUpdates, info.NewMessages);
            ExecuteDeleteByIds(db.MessageStatusUpdates, info.ReadedMessages);
            ExecuteDeleteByIds(db.ChatUpdates, info.ChatsChanged);
            ExecuteDeleteByIds(db.ChatInviteUpdates, info.ChatInvites);
            ExecuteDeleteByIds(db.FriendRequestUpdates, info.FriendRequests);
            ExecuteDeleteByIds(db.ChatListUpdates, info.ChatListUpdates);
            ExecuteDeleteByIds(db.UserProfileUpdates, info.RelatedUsersChanged);
            ExecuteDeleteByIds(db.FriendListUpdates, info.FriendListUpdates);
            db.SaveChanges();
        }

        private void ExecuteDeleteByIds<T>(DbSet<T> dbSet, int[] ids) where T : Update
        {
            dbSet.Where(x => ids.Contains(x.Id)).ExecuteDelete();
        }

        public bool IsHaveUpdates(int userId)
        {
            using var db = new TmdbContext();
            return db.NewMessageUpdates.Any(u => u.UserId == userId) ||
                   db.MessageStatusUpdates.Any(u => u.UserId == userId) ||

                   db.ChatInviteUpdates.Any(u => u.UserId == userId) ||
                   db.ChatListUpdates.Any(u => u.UserId == userId) ||
                   db.ChatUpdates.Any(u => u.UserId == userId) ||

                   db.UserProfileUpdates.Any(u => u.UserId == userId) ||

                   db.FriendRequestUpdates.Any(u => u.UserId == userId) ||
                   db.FriendListUpdates.Any(u => u.UserId == userId);
        }
    }
}
