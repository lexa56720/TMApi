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
    public class Friends
    {
        private readonly Chats Chats;

        public Friends(Chats chats)
        {
            Chats = chats;
        }
        public async Task<DBFriendRequest[]> GetFriendRequest(int[] ids)
        {
            using var db = new TmdbContext();
            return await db.FriendRequests.Where(r => ids.Contains(r.Id))
                                          .ToArrayAsync();
        }

        public async Task<DBFriendRequest> RemoveFriendRequest(int requestId)
        {
            using var db = new TmdbContext();
            var request = await db.FriendRequests.SingleAsync(r => r.Id == requestId);
            db.FriendRequests.Remove(request);
            await db.SaveChangesAsync();
            return request;
        }
        public async Task<DBFriendRequest> RemoveFriendRequest(int fromId, int toId)
        {
            using var db = new TmdbContext();
            var request = await db.FriendRequests.SingleAsync(r => r.SenderId == fromId && r.ReceiverId == toId);
            db.FriendRequests.Remove(request);
            await db.SaveChangesAsync();
            return request;
        }
        public async Task RegisterFriendRequest(int fromId, int toId)
        {
            using var db = new TmdbContext();

            await db.FriendRequests.AddAsync(new DBFriendRequest()
            {
                SenderId = fromId,
                ReceiverId = toId,
            });
            await db.SaveChangesAsync(true);
        }

        public async Task RegisterFriends(int senderId, int responderId)
        {
            using var db = new TmdbContext();
            await db.Friends.AddAsync(new DBFriend()
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
            await db.Chats.AddAsync(chat);
            chat.Members.Add(await db.Users.FindAsync(senderId));
            chat.Members.Add(await db.Users.FindAsync(responderId));

            await db.SaveChangesAsync(true);
        }

        public async Task<int[]> GetAllForUser(int userId)
        {
            using var db = new TmdbContext();
            return await db.FriendRequests.Where(i => i.ReceiverId == userId)
                                          .Select(i => i.Id)
                                          .ToArrayAsync();
        }

        public async Task<bool> RemoveFriend(int userId, int friendId)
        {
            using var db = new TmdbContext();
            var friend = await db.Friends.SingleAsync(f => (f.DestId == userId && f.SenderId == friendId)
                                                        || (f.SenderId == userId && f.DestId == friendId));
            db.Friends.Remove(friend);
            var dialogue = await db.Chats.Include(c => c.Members)
                                        .SingleAsync(c => c.IsDialogue
                                              && (c.Members.ElementAt(0).Id == userId && c.Members.ElementAt(1).Id == friendId
                                              || (c.Members.ElementAt(1).Id == userId && c.Members.ElementAt(0).Id == friendId)));

            await Chats.RemoveChat(dialogue.Id, db);
            return await db.SaveChangesAsync(true) > 0;
        }
    }
}
