using ApiTypes.Communication.LongPolling;
using Microsoft.EntityFrameworkCore;
using TMServer.DataBase.Tables.LongPolling;
using TMServer.ServerComponent.LongPolling;

namespace TMServer.DataBase.Interaction
{
    public class LongPolling
    {
        public async Task<DBNewMessageUpdate[]> GetMessageUpdate(int userId)
        {
            using var db = new TmdbContext();
            return await db.NewMessageUpdates.Where(c => c.UserId == userId)
                                             .ToArrayAsync();
        }

        public async Task<DBMessageStatusUpdate[]> GetMessagesWithUpdatedStatus(int userId)
        {
            using var db = new TmdbContext();
            return await db.MessageStatusUpdates.Where(c => c.UserId == userId)
                                                .ToArrayAsync();
        }
        public async Task<DBFriendRequestUpdate[]> GetFriendRequestUpdates(int userId)
        {
            using var db = new TmdbContext();
            return await db.FriendRequestUpdates.Where(r => r.UserId == userId)
                                                .ToArrayAsync();
        }
        public async Task<DBFriendListUpdate[]> GetFriendListUpdates(int userId)
        {
            using var db = new TmdbContext();
            return await db.FriendListUpdates.Where(fl => fl.UserId == userId)
                                             .ToArrayAsync();
        }

        public async Task<DBChatListUpdate[]> GetChatListUpdates(int userId)
        {
            using var db = new TmdbContext();
            return await db.ChatListUpdates.Where(cu => cu.UserId == userId)
                                           .ToArrayAsync();
        }

        public async Task<DBUserProfileUpdate[]> GetRelatedUsersUpdates(int userId)
        {
            using var db = new TmdbContext();
            return await db.UserProfileUpdates.Where(u => u.UserId == userId)
                                              .ToArrayAsync();
        }

        public async Task<DBChatInviteUpdate[]> GetChatInvites(int userId)
        {
            using var db = new TmdbContext();
            return await db.ChatInviteUpdates.Where(c => c.UserId == userId)
                                             .ToArrayAsync();
        }

        public async Task<DBChatUpdate[]> GetChatUpdates(int userId)
        {
            using var db = new TmdbContext();
            return await db.ChatUpdates.Where(c => c.UserId == userId)
                                       .ToArrayAsync();
        }

        public async Task<DBUserOnlineUpdate[]> GetOnlineUpdates(int userId)
        {
            using var db = new TmdbContext();
            return await db.UserOnlineUpdates.Where(c => c.UserId == userId)
                                             .ToArrayAsync();
        }


        public async Task ClearAllUpdates(int userId)
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
            db.UserOnlineUpdates.Where(c => c.UserId == userId).ExecuteDelete();
            await db.SaveChangesAsync();
        }
        public async Task ClearUpdatesByIds(LongPollResponseInfo info)
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
            ExecuteDeleteByIds(db.UserOnlineUpdates, info.OnlineUpdates);
            await db.SaveChangesAsync();
        }

        private void ExecuteDeleteByIds<T>(DbSet<T> dbSet, int[] ids) where T : Update
        {
            dbSet.Where(x => ids.Contains(x.Id)).ExecuteDelete();
        }

    }
}
