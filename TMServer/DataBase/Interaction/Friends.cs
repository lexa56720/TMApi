using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Friends;
using ApiTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Tables;
using System.Xml.Linq;

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
                (r => r.Id == responseId);

            if (request != null)
            {
                db.FriendRequests.Remove(request);
                if (isAccepted)
                    RegisterFriends(request.SenderId, request.ReceiverId);

                db.SaveChanges();
                return;
            }
        }
        public static void RegisterFriendRequest(int fromId, int toId)
        {
            using var db = new TmdbContext();
            if (IsAlreadyRequested(fromId, toId) || IsAlreadyFriends(fromId, toId))
            {
                RegisterFriends(fromId, toId);
                return;
            }

            db.FriendRequests.Add(new DBFriendRequest()
            {
                SenderId = fromId,
                ReceiverId = toId,
            });
            db.SaveChanges();
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

                var admin = db.Users.Find(idOne);
                var chat = new DBChat()
                {
                    AdminId=idOne,
                    IsDialogue = true,
                    Name = string.Empty,
                };
                db.Chats.Add(chat);
                chat.Members.Add(admin);
                chat.Members.Add(db.Users.Find(idTwo));
                db.SaveChanges();          
            }
        }

        public static int[] GetAllForUser(int userId)
        {
            using var db = new TmdbContext();
            return db.FriendRequests.Where(i => i.ReceiverId == userId)
                                    .Select(i => i.Id).ToArray();
        }
        private static bool IsAlreadyRequested(int idOne, int idTwo)
        {
            using var db = new TmdbContext();
            return db.FriendRequests.Any(r => (r.SenderId == idOne && r.ReceiverId == idTwo)
                                           || (r.SenderId == idTwo && r.ReceiverId == idOne));
        }
        private static bool IsAlreadyFriends(int idOne, int idTwo)
        {
            using var db = new TmdbContext();
            return db.Friends.Any(f => (f.SenderId == idOne && f.DestId == idTwo)
                                    || (f.SenderId == idTwo && f.DestId == idOne));
        }
    }
}
