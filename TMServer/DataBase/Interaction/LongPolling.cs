using ApiTypes.Communication.LongPolling;
using Microsoft.EntityFrameworkCore;

namespace TMServer.DataBase.Interaction
{
    internal static class LongPolling
    {
        public static int[] GetMessageUpdate(int userId)
        {
            using var db = new TmdbContext();
            var result = db.NewMessages
                           .Where(c => c.UserId == userId)
                           .Select(m => m.MessageId)
                           .ToArray();
            db.NewMessages.Where(u => u.UserId == userId)
                             .ExecuteDelete();
            db.SaveChanges();
            return result;
        }

        public static int[] GetMessagesWithUpdatedStatus(int userId)
        {
            using var db = new TmdbContext();
            var result = db.MessageStatusUpdates
                           .Where(c => c.UserId == userId)
                           .Select(m => m.MessageId)
                           .ToArray();

            db.MessageStatusUpdates.Where(u => u.UserId == userId)
                                   .ExecuteDelete();
            db.SaveChanges();
            return result;
        }
        public static int[] GetFriendRequestUpdates(int userId)
        {
            using var db = new TmdbContext();
            var result = db.FriendRequestUpdates
                            .Where(r => r.UserId == userId)
                            .Select(r => r.RequestId)
                            .ToArray();

            db.FriendRequestUpdates.Where(r => r.UserId == userId)
                                   .ExecuteDelete();
            db.SaveChanges();
            return result;
        }
        public static int[] GetNewFriends(int userId)
        {
            using var db = new TmdbContext();
            var result = db.FriendListUpdates
                .Where(fl => fl.UserId == userId && fl.IsAdded)
                .Select(fl => fl.FriendId)
                .ToArray();

            db.FriendListUpdates.Where(fl => fl.UserId == userId && fl.IsAdded)
                               .ExecuteDelete();
            db.SaveChanges();
            return result;
        }
        public static int[] GetRemovedFriends(int userId)
        {
            using var db = new TmdbContext();
            var result = db.FriendListUpdates
                .Where(fl => fl.UserId == userId && !fl.IsAdded)
                .Select(fl => fl.FriendId)
                .ToArray();

            db.FriendListUpdates.Where(fl => fl.UserId == userId && !fl.IsAdded)
                               .ExecuteDelete();
            db.SaveChanges();
            return result;
        }
        public static int[] GetNewChats(int userId)
        {
            using var db = new TmdbContext();
            var result = db.ChatUpdates
                .Where(cu => cu.UserId == userId && cu.IsAdded)
                .Select(fl => fl.ChatId)
                .ToArray();

            db.ChatUpdates.Where(cu => cu.UserId == userId && cu.IsAdded)
                          .ExecuteDelete();
            db.SaveChanges();
            return result;
        }

        public static void ClearUpdates(int userId)
        {
            using var db = new TmdbContext();
            db.NewMessages.Where(u => u.UserId == userId).ExecuteDelete();
            db.MessageStatusUpdates.Where(u => u.UserId == userId).ExecuteDelete();
            db.FriendRequestUpdates.Where(fr => fr.UserId == userId).ExecuteDelete();
            db.ChatUpdates.Where(c => c.UserId == userId).ExecuteDelete();
            db.FriendProfileUpdates.Where(p => p.UserId == userId).ExecuteDelete();
            db.FriendListUpdates.Where(fl => fl.UserId == userId).ExecuteDelete();
            db.SaveChanges();
        }

        public static bool IsHaveUpdates(int userId)
        {
            using var db = new TmdbContext();
            return db.NewMessages.Any(u => u.UserId == userId) ||
                   db.MessageStatusUpdates.Any(u => u.UserId == userId) ||
                   db.FriendRequestUpdates.Any(u => u.UserId == userId) ||
                   db.ChatInviteUpdates.Any(u => u.UserId == userId) ||
                   db.ChatUpdates.Any(u => u.UserId == userId) ||
                   db.FriendProfileUpdates.Any(u => u.UserId == userId) ||
                   db.FriendListUpdates.Any(u => u.UserId == userId);
        }
    }
}
