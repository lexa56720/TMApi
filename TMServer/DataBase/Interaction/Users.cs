using ApiTypes.Communication.Users;
using Microsoft.EntityFrameworkCore;
using PerformanceUtils.Collections;
using TMServer.DataBase.Tables;
using TMServer.DataBase.Tables.LongPolling;

namespace TMServer.DataBase.Interaction
{
    public class Users : IDisposable
    {
        private readonly LifeTimeDictionary<int, int> OnlineUsers;
        private readonly TimeSpan OnlineTimeout;
        private bool IsDisposed = false;
        public Users(TimeSpan onlineTimeout)
        {
            OnlineUsers = new(SetOffline);
            OnlineTimeout = onlineTimeout;
        }
        public void Dispose()
        {
            if (IsDisposed)
                return;

            OnlineUsers.Clear();
            IsDisposed = true;
        }
        public async Task<DBUser?> GetUser(int id)
        {
            using var db = new TmdbContext();
            return await db.Users.SingleOrDefaultAsync(u => u.Id == id);
        }
        public async Task<DBUser?> GetUserWithFriends(int id)
        {
            using var db = new TmdbContext();
            return await db.Users
                .Include(u => u.Chats)
                .Include(u => u.FriendsTwo).ThenInclude(f => f.Sender)
                .Include(u => u.FriendsOne).ThenInclude(f => f.Receiver)
                .SingleOrDefaultAsync(u => u.Id == id);
        }

        public static async Task<int[]> GetAllRelatedUsers(int userId)
        {
            using var db = new TmdbContext();
            var user = await db.Users
                .Include(u => u.Chats)
                .Include(u => u.FriendsTwo).ThenInclude(f => f.Sender)
                .Include(u => u.FriendsOne).ThenInclude(f => f.Receiver)
                .SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return [];

            var chatMembers = user.Chats.SelectMany(c => c.Members.Where(m => m.Id != user.Id)
                                                      .Select(m => m.Id));

            var invited = db.ChatInvites.Where(i => i.InviterId == user.Id)
                                        .Select(i => i.ToUserId);

            var requested = db.FriendRequests.Where(r => r.SenderId == user.Id)
                                             .Select(r => r.ReceiverId);

            return chatMembers.Union(await invited.ToArrayAsync())
                              .Union(await requested.ToArrayAsync())
                              .Union(user.GetFriends().Select(f => f.Id))
                              .ToArray();
        }
        public async Task<DBUser[]> GetUserMain(int[] ids)
        {
            using var db = new TmdbContext();

            var users = db.Users.Where(u => ids.Contains(u.Id));
            return await users.ToArrayAsync();
        }

        public async Task<DBUser[]> GetUserByName(string name)
        {
            using var db = new TmdbContext();

            return await db.Users.Where(u => u.Name.Contains(name))
                                 .Take(20)
                                 .ToArrayAsync();
        }
        public async Task<DBUser[]> GetUserByLogin(string login)
        {
            using var db = new TmdbContext();

            return await db.Users.Where(u => u.Login.Contains(login))
                                 .Take(20)
                                 .ToArrayAsync();
        }

        public async Task<(DBUser? user, int prevSetId)> SetProfileImage(int userId, int imageId)
        {
            using var db = new TmdbContext();

            var user = await db.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return (null, -1);

            var prevSetId = user.ProfileImageId;
            user.ProfileImageId = imageId;
            await UpdateProfile(userId, db);
            await db.SaveChangesAsync(true);
            return (user, prevSetId);
        }
        public async Task<DBUser?> ChangeName(int userId, string newName)
        {
            using var db = new TmdbContext();

            var user = await db.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return null;

            user.Name = newName;
            await UpdateProfile(userId, db);
            await db.SaveChangesAsync(true);
            return user;
        }

        public async Task UpdateOnlineStatus(int userId)
        {
            if (OnlineUsers.TryGetValue(userId, out userId))
            {
                OnlineUsers.UpdateLifetime(userId, OnlineTimeout);
            }
            else
            {
                using var db = new TmdbContext();
                OnlineUsers.TryAdd(userId, userId, OnlineTimeout);
                await StatusUpdate(userId, true, db);
                await db.SaveChangesAsync(true);
            }
        }

        private async Task SetOffline(int userId)
        {
            using var db = new TmdbContext();
            var user = db.Users.SingleOrDefault(u => u.Id == userId);
            if (user == null)
                return;
            user.LastRequest = DateTime.UtcNow;
            await StatusUpdate(userId, false, db);
            await db.SaveChangesAsync(true);
        }

        private async Task UpdateProfile(int userId, TmdbContext db)
        {
            var related = await GetAllRelatedUsers(userId);
            foreach (var relatedUser in related)
            {
                await db.UserProfileUpdates.AddAsync(new DBUserProfileUpdate()
                {
                    UserId = relatedUser,
                    ProfileId = userId,
                });
            }
        }

        private async Task StatusUpdate(int userId, bool isOnline, TmdbContext db)
        {
            var related = await GetAllRelatedUsers(userId);
            foreach (var relatedUser in related)
            {
                await db.UserOnlineUpdates.AddAsync(new DBUserOnlineUpdate()
                {
                    Date = DateTime.UtcNow,
                    IsAdded = isOnline,
                    RelatedUserId = userId,
                    UserId = relatedUser
                });
            }
        }

    }
}
