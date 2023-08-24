using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public static DBUser[] GetUsers(int[] ids)
        {
            using var db = new TmdbContext();

            var users = db.Users.Where(u => ids.Contains(u.Id));
            return users.ToArray();
        }
    }
}
