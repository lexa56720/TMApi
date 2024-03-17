using ApiTypes.Communication.Chats;
using ApiTypes.Communication.Friends;
using ApiTypes.Communication.Messages;
using ApiTypes.Communication.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Tables;

namespace TMServer.RequestHandlers
{
    public static class DbConverter
    {
        public static Chat Convert(DBChat chat, int unreadCount)
        {
            return new Chat()
            {
                Id = chat.Id,
                AdminId = chat.Admin.Id,
                Name = chat.Name,
                MemberIds = chat.Members.Select(m => m.Id)
                                        .ToArray(),
                UnreadCount = unreadCount,
                IsDialogue = chat.IsDialogue,
            };
        }
        public static Chat[] Convert(DBChat[] chats, int[] unreadCounts)
        {
            var result = new Chat[chats.Length];
            for (int i = 0; i < chats.Length; i++)
                result[i] = Convert(chats[i], unreadCounts[i]);
            return result;
        }

        public static Message Convert(DBMessage dbMessage, bool isReaded)
        {
            return new Message(dbMessage.Id, dbMessage.AuthorId, dbMessage.DestinationId,
                                             dbMessage.Content, dbMessage.SendTime, isReaded);
        }
        public static Message[] Convert(DBMessage[] dbMessages, bool[] isReaded)
        {
            var result = new Message[dbMessages.Length];
            for (int i = 0; i < dbMessages.Length; i++)
                result[i] = Convert(dbMessages[i], isReaded[i]);

            return result;
        }

        public static User Convert(DBUser dBUser)
        {
            return new User()
            {
                Name = dBUser.Name,
                Id = dBUser.Id,
                Login = dBUser.Login,
                IsOnline = dBUser.IsOnline
            };
        }
        public static User[] Convert(DBUser[] users)
        {
            var result = new User[users.Length];
            for (int i = 0; i < users.Length; i++)
                result[i] = Convert(users[i]);
            return result;
        }

        public static FriendRequest Convert(DBFriendRequest request)
        {
            return new FriendRequest(request.ReceiverId, request.SenderId, request.Id);
        }
    }
}
