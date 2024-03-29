using Microsoft.EntityFrameworkCore;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase.Interaction
{
    internal static class Users
    {
        public static DBUser? GetUser(int id)
        {
            using var db = new TmdbContext();
            return db.Users.SingleOrDefault(u => u.Id == id);
        }
        public static DBUser? GetUserFull(int id)
        {
            using var db = new TmdbContext();
            return db.Users
                .Include(u => u.Chats)
                .Include(u => u.FriendsTwo).ThenInclude(f => f.Sender)
                .Include(u => u.FriendsOne).ThenInclude(f => f.Receiver)
                .SingleOrDefault(u => u.Id == id);
        }
        public static DBUser[] GetUserMain(int[] ids)
        {
            using var db = new TmdbContext();

            var users = db.Users.Where(u => ids.Contains(u.Id));
            return users.ToArray();
        }

        public static DBUser[] GetUserByName(string name)
        {
            using var db = new TmdbContext();

            return db.Users
                .Where(u => u.Name.Contains(name))
                .Take(20).ToArray();
        }
        public static DBUser[] GetUserByLogin(string login)
        {
            using var db = new TmdbContext();

            return db.Users
                .Where(u => u.Login.Contains(login))
                .Take(20).ToArray();
        }

        public static DBUser? SetProfileImage(int userId, int imageId,out int prevSetId)
        {
            using var db = new TmdbContext();

            var user = db.Users.Single(u => u.Id == userId);
            prevSetId = user.ProfileImageId;
            user.ProfileImageId = imageId;
            db.SaveChanges(true);
            return user;
        }
        public static DBUser? ChangeName(int userId, string newName)
        {
            using var db = new TmdbContext();

            var user = db.Users.SingleOrDefault(u => u.Id == userId);
            if (user == null)
                return null;

            user.Name = newName;
            db.SaveChanges();
            return user;
        }

        public static void UpdateLastRequest(int userId)
        {
            using var db = new TmdbContext();
            var user = db.Users.SingleOrDefault(u=>u.Id==userId);
            if (user == null)
                return;

            user.LastRequest = DateTime.UtcNow;
            db.SaveChanges();
        }
    }
}
