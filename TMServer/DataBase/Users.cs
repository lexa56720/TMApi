using Microsoft.EntityFrameworkCore;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase
{
    internal static class Users
    {
        public static DBUser? GetUserFull(int id)
        {
            using var db = new TmdbContext();
            return db.Users
                .Include(u => u.Chats)
                .Include(u => u.FriendsTwo)
                .Include(u => u.FriendsOne)
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
