using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Friends;
using ApiTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Tables;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TMServer.DataBase
{
    internal class Friends
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
            if (IsFriendshipPossible(fromId, toId))
            {
                db.FriendRequests.Add(new DBFriendRequest()
                {
                    UserOneId = fromId,
                    UserTwoId = toId,
                });
                db.SaveChanges();
            }

        }

        private static bool IsFriendshipPossible(int idOne, int idTwo)
        {
            using var db = new TmdbContext();
            if (db.FriendRequests.Any(r => (r.UserOneId == idOne && r.UserTwoId == idTwo)
                                        || (r.UserTwoId == idTwo && r.UserOneId == idOne)))
                return false;

            if (db.Friends.Any(f => (f.UserIdOne == idOne && f.UserIdTwo == idTwo)
                              || (f.UserIdOne == idTwo && f.UserIdTwo == idOne)))
                return false;

            return true;
        }
        private static void RegisterFriends(int idOne, int idTwo)
        {
            using var db = new TmdbContext();
            if (IsFriendshipPossible(idOne, idTwo))
                db.Friends.Add(new DBFriend()
                {
                    UserIdOne = idOne,
                    UserIdTwo = idTwo
                });
            db.SaveChanges();
            Chats.CreateChat(string.Empty,idOne, idTwo);
        }
    }
}
