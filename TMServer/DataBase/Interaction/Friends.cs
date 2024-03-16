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
        public static DBFriendRequest[] GetFriendRequest(int[] ids)
        {
            using var db = new TmdbContext();
            var requests = db.FriendRequests.Where(r => ids.Contains(r.Id));

            return requests.ToArray();
        }

        public static void FriendRequestResponse(int requestId, bool isAccepted)
        {
            using var db = new TmdbContext();
            var request = db.FriendRequests.SingleOrDefault(r => r.Id == requestId);
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

            var oppositeRequest = db.FriendRequests.SingleOrDefault(r => r.ReceiverId == fromId && r.SenderId == toId);
            if (oppositeRequest != null)
            {
                db.FriendRequests.Remove(oppositeRequest);
                RegisterFriends(toId,fromId);
                db.SaveChanges(true);
                return;
            }

            db.FriendRequests.Add(new DBFriendRequest()
            {
                SenderId = fromId,
                ReceiverId = toId,
            });
            db.SaveChanges(true);
        }

        private static void RegisterFriends(int senderId, int responderId)
        {
            if (Security.IsAlreadyFriends(senderId, responderId))
                return;

            using var db = new TmdbContext();
            db.Friends.Add(new DBFriend()
            {
                SenderId = senderId,
                DestId = responderId
            });

            var chat = new DBChat()
            {
                AdminId = senderId,
                IsDialogue = true,
                Name = string.Empty,
            };
            db.Chats.Add(chat);
            chat.Members.Add(db.Users.Find(senderId));
            chat.Members.Add(db.Users.Find(responderId));
            db.SaveChanges(true);
        }

        public static int[] GetAllForUser(int userId)
        {
            using var db = new TmdbContext();
            return db.FriendRequests.Where(i => i.ReceiverId == userId)
                                    .Select(i => i.Id)
                                    .ToArray();
        }

    }
}
