using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Friends;
using ApiTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase.Interaction
{
    internal static class Friends
    {
        public static DBFriendRequest? GetFriendRequest(int id)
        {
            using var db = new TmdbContext();
            return db.FriendRequests.Find(id);
        }
        public static DBFriendRequest[] GetFriendRequest(int[] ids)
        {
            using var db = new TmdbContext();
            var requests = db.FriendRequests.Where(r => ids.Contains(r.Id));

            return requests.ToArray();
        }

        public static void FriendRequestResponse(int responseId, bool isAccepted)
        {
            using var db = new TmdbContext();
            var request = db.FriendRequests.SingleOrDefault
                (r => r.Id== responseId);

            if (request != null)
            {
                db.FriendRequests.Remove(request);
                if (isAccepted)
                    RegisterFriends(request.UserOneId, request.UserTwoId);

                db.SaveChanges();
                return;
            }
        }
        public static void RegisterFriendRequest(int fromId, int toId)
        {
            using var db = new TmdbContext();
            if (!IsAlreadyRequested(fromId, toId) && !IsAlreadyFriends(fromId,toId))
            {
                db.FriendRequests.Add(new DBFriendRequest()
                {
                    UserOneId = fromId,
                    UserTwoId = toId,
                });
                db.SaveChanges();
            }
        }

        private static void RegisterFriends(int idOne, int idTwo)
        {
            using var db = new TmdbContext();
            if (!IsAlreadyFriends(idOne, idTwo))
            {
                db.Friends.Add(new DBFriend()
                {
                    SenderId = idOne,
                    DestId = idTwo
                });
                db.SaveChanges();
                Chats.CreateChat(string.Empty,true ,idOne, idTwo);
            }
        }
              
        public static int[] GetAllForUser(int userId)
        {
            using var db = new TmdbContext();
            return db.FriendRequests.Where(i => i.UserTwoId == userId)
                                    .Select(i => i.Id).ToArray();
        }
        private static bool IsAlreadyRequested(int idOne, int idTwo)
        {
            using var db = new TmdbContext();
            return db.FriendRequests.Any(r => (r.UserOneId == idOne && r.UserTwoId == idTwo)
                                           || (r.UserOneId == idTwo && r.UserTwoId == idOne));
        }
        private static bool IsAlreadyFriends(int idOne, int idTwo)
        {
            using var db = new TmdbContext();
            return db.Friends.Any(f => (f.SenderId == idOne && f.DestId == idTwo)
                                    || (f.SenderId == idTwo && f.DestId == idOne));
        }
    }
}
