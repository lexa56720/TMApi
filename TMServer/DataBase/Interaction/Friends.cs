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
using Microsoft.EntityFrameworkCore;

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

        public static DBFriendRequest RemoveFriendRequest(int requestId)
        {
            using var db = new TmdbContext();
            var request = db.FriendRequests.Single(r => r.Id == requestId);
            db.FriendRequests.Remove(request);
            db.SaveChanges();
            return request;
        }
        public static DBFriendRequest RemoveFriendRequest(int fromId, int toId)
        {
            using var db = new TmdbContext();
            var request = db.FriendRequests.Single(r => r.SenderId == fromId && r.ReceiverId == toId);
            db.FriendRequests.Remove(request);
            db.SaveChanges();
            return request;
        }
        public static void RegisterFriendRequest(int fromId, int toId)
        {
            using var db = new TmdbContext();

            db.FriendRequests.Add(new DBFriendRequest()
            {
                SenderId = fromId,
                ReceiverId = toId,
            });
            db.SaveChanges(true);
        }

        public static void RegisterFriends(int senderId, int responderId)
        {
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
