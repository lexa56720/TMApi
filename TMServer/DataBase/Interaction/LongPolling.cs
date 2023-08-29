using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Interaction
{
    internal static class LongPolling
    {
        public static void GetUpdate(int userId)
        {
            //using var db = new TmdbContext();

            //var user = db.Users
            //    .Include(u => u.FriendsOne)
            //    .Include(u => u.FriendsTwo)
            //    .Include(u => u.Chats)
            //    .SingleOrDefault(u => u.Id == userId);

            //db.ChangeTracker.HasChanges();

        }
    }
}
