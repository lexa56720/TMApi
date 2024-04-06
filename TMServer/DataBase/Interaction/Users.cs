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

        private bool IsDisposed = false;
        public Users()
        {
            OnlineUsers = new(SetOffline);
        }
        public void Dispose()
        {
            if (IsDisposed)
                return;

            OnlineUsers.Clear();
            IsDisposed = true;
        }
        public DBUser? GetUser(int id)
        {
            using var db = new TmdbContext();
            return db.Users.SingleOrDefault(u => u.Id == id);
        }
        public DBUser? GetUserWithFriends(int id)
        {
            using var db = new TmdbContext();
            return db.Users
                .Include(u => u.Chats)
                .Include(u => u.FriendsTwo).ThenInclude(f => f.Sender)
                .Include(u => u.FriendsOne).ThenInclude(f => f.Receiver)
                .SingleOrDefault(u => u.Id == id);
        }

        public static int[] GetAllRelatedUsers(int userId)
        {
            using var db = new TmdbContext();
            var user = db.Users.Include(u => u.Chats)
                              .Include(u => u.FriendsTwo).ThenInclude(f => f.Sender)
                              .Include(u => u.FriendsOne).ThenInclude(f => f.Receiver)
                              .SingleOrDefault(u => u.Id == userId);
            if (user == null)
                return [];

            var chatMembers = user.Chats.SelectMany(c => c.Members.Where(m => m.Id != user.Id)
                                                      .Select(m => m.Id));

            var invited = db.ChatInvites.Where(i => i.InviterId == user.Id)
                                        .Select(i => i.ToUserId);

            var requested = db.FriendRequests.Where(r => r.SenderId == user.Id)
                                             .Select(r => r.ReceiverId);

            return chatMembers.Union(invited)
                              .Union(requested)
                              .Union(user.GetFriends().Select(f => f.Id))
                              .ToArray();
        }
        public DBUser[] GetUserMain(int[] ids)
        {
            using var db = new TmdbContext();

            var users = db.Users.Where(u => ids.Contains(u.Id));
            return users.ToArray();
        }

        public DBUser[] GetUserByName(string name)
        {
            using var db = new TmdbContext();

            return db.Users
                .Where(u => u.Name.Contains(name))
                .Take(20).ToArray();
        }
        public DBUser[] GetUserByLogin(string login)
        {
            using var db = new TmdbContext();

            return db.Users
                .Where(u => u.Login.Contains(login))
                .Take(20).ToArray();
        }

        public DBUser? SetProfileImage(int userId, int imageId, out int prevSetId)
        {
            using var db = new TmdbContext();

            var user = db.Users.SingleOrDefault(u => u.Id == userId);
            if (user == null)
            {
                prevSetId = -1;
                return null;
            }
            prevSetId = user.ProfileImageId;
            user.ProfileImageId = imageId;
            db.SaveChanges(true);
            return user;
        }
        public DBUser? ChangeName(int userId, string newName)
        {
            using var db = new TmdbContext();

            var user = db.Users.SingleOrDefault(u => u.Id == userId);
            if (user == null)
                return null;

            user.Name = newName;
            db.SaveChanges();
            return user;
        }

        public void UpdateOnlineStatus(int userId)
        {
            using var db = new TmdbContext();
            var user = db.Users.SingleOrDefault(u => u.Id == userId);
            if (user == null)
                return;
            var prevOnlineState = user.IsOnline;
            user.LastRequest = DateTime.UtcNow;
            if (user.IsOnline == prevOnlineState)
            {
                db.SaveChanges();
                return;
            }

            if (!OnlineUsers.TryAdd(userId, userId, GlobalSettings.OnlineTimeout))
                OnlineUsers.UpdateLifetime(userId, GlobalSettings.OnlineTimeout);

            StatusUpdate(userId, user.IsOnline, db);
            db.SaveChanges(true);
        }

        private void SetOffline(int userId)
        {
            using var db = new TmdbContext();
            var user = db.Users.SingleOrDefault(u => u.Id == userId);
            if (user == null)
                return;
            db.Entry(user).State = EntityState.Modified;
            StatusUpdate(userId, false, db);
            db.SaveChanges(true);
        }

        private void StatusUpdate(int userId, bool isOnline, TmdbContext db)
        {
            var related = GetAllRelatedUsers(userId);
            foreach (var relatedUser in related)
            {
                db.UserOnlineUpdates.Where(u => u.UserId == userId && u.RelatedUserId == relatedUser)
                                    .ExecuteDelete();
                db.UserOnlineUpdates.Add(new DBUserOnlineUpdate()
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
