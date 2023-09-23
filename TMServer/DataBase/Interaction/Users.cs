using Microsoft.EntityFrameworkCore;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase.Interaction
{
    internal static class Users
    {
        public static DBUser? GetUserFull(int id)
        {
            using var db = new TmdbContext();
            return db.Users
                .Include(u => u.Chats)
                .Include(u => u.FriendsTwo).ThenInclude(f=>f.UserOne)
                .Include(u => u.FriendsOne).ThenInclude(f => f.UserTwo)
                .SingleOrDefault(u => u.Id == id);
        }
        public static DBUser? GetUserMain(int id)
        {
            using var db = new TmdbContext();
            return db.Users.SingleOrDefault(u => u.Id == id);
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
        public static void ChangeName(int userId, string newName)
        {
            using var db = new TmdbContext();

            var user = db.Users.SingleOrDefault(u => u.Id == userId);
            if (user == null)
                return;

            user.Name = newName;
            db.SaveChanges();
        }
    }
}
