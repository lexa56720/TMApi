using ApiTypes.Communication.Chats;
using ApiTypes.Communication.Friends;
using ApiTypes.Communication.Medias;
using ApiTypes.Communication.Messages;
using ApiTypes.Communication.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Interaction;
using TMServer.DataBase.Tables;


using ApiImageSize = ApiTypes.Communication.Medias.ImageSize;
using DBImageSize = TMServer.DataBase.Tables.ImageSize;

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
            if (dbMessage.IsSystem && dbMessage.Action != null)
            {
                return new Message(dbMessage.Id, dbMessage.AuthorId, dbMessage.DestinationId,
                                 dbMessage.Content, dbMessage.SendTime, isReaded, dbMessage.Action.Kind,
                                 dbMessage.Action.ExecutorId, dbMessage.Action.TargetId == null ? -1 : dbMessage.Action.TargetId.Value);
            }

            var images = Images.GetImage(dbMessage.Medias.Select(m => m.MediaId).ToArray());
            return new Message(dbMessage.Id, dbMessage.AuthorId, dbMessage.DestinationId,
                               dbMessage.Content, dbMessage.SendTime, isReaded, Convert(images));
        }
        public static Message[] Convert(DBMessage[] dbMessages, bool[] isReaded)
        {
            var result = new Message[dbMessages.Length];
            for (int i = 0; i < dbMessages.Length; i++)
                result[i] = Convert(dbMessages[i], isReaded[i]);

            return result;
        }

        public static User Convert(DBUser dBUser, DBImageSet? profilePic)
        {
            Photo[] pics=[];
            if (profilePic != null)
                pics = Convert(profilePic);

            return new User()
            {
                Name = dBUser.Name,
                Id = dBUser.Id,
                Login = dBUser.Login,
                IsOnline = dBUser.IsOnline,
                LastAction=dBUser.LastRequest,
                ProfilePics = pics,
            };
        }
        public static User[] Convert(DBUser[] users)
        {
            var profilePics = Images.GetImageSet(users.Select(u => u.ProfileImageId).ToArray());
            var result = new User[users.Length];
            for (int i = 0; i < users.Length; i++)
                result[i] = Convert(users[i], profilePics[i]);
            return result;
        }

        public static ChatInvite Convert(DBChatInvite inivite)
        {
            return new ChatInvite(inivite.ChatId,
                                  inivite.ToUserId,
                                  inivite.InviterId,
                                  inivite.Id);
        }
        public static ChatInvite[] Convert(DBChatInvite[] invites)
        {
            var result = new ChatInvite[invites.Length];
            for (int i = 0; i < invites.Length; i++)
                result[i] = Convert(invites[i]);
            return result;
        }

        public static FriendRequest Convert(DBFriendRequest request)
        {
            return new FriendRequest(request.ReceiverId, request.SenderId, request.Id);
        }
        public static FriendRequest[] Convert(DBFriendRequest[] requests)
        {
            var result = new FriendRequest[requests.Length];
            for (int i = 0; i < requests.Length; i++)
                result[i] = Convert(requests[i]);
            return result;
        }


        public static Photo[] Convert(DBImageSet set)
        {
            return Convert(set.Images.ToArray());
        }
        public static Photo[] Convert(DBImage[] images)
        {
            var result = new Photo[images.Length];
            for (int i = 0; i < result.Length; i++)
            {
                var url = $"images/{images[i].Url}/{images[i].Id}";
                var size = Convert(images[i].Size);
                result[i] = new Photo(url, size);
            }
            return result;
        }
        private static ApiImageSize Convert(DBImageSize imageSize)
        {
            return imageSize switch
            {
                DBImageSize.Small => ApiImageSize.Small,
                DBImageSize.Medium => ApiImageSize.Medium,
                DBImageSize.Large => ApiImageSize.Large,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
