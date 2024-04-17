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
using TMServer.DataBase.Tables.FileTables;
using ApiImageSize = ApiTypes.Communication.Medias.ImageSize;
using DBImageSize = TMServer.DataBase.Tables.FileTables.ImageSize;

namespace TMServer.RequestHandlers
{
    public class DbConverter
    {
        private readonly Files Files;

        public DbConverter(Files files)
        {
            Files = files;
        }

        public async Task<Chat> Convert(DBChat chat, int unreadCount)
        {
            return Convert(chat, unreadCount, await Files.GetImageSetWithoutData(chat.CoverImageId));
        }
        public Chat Convert(DBChat chat, int unreadCount, DBImageSet? cover)
        {
            PhotoLink[] coverPics = [];
            if (cover != null)
                coverPics = Convert(cover);

            return new Chat()
            {
                Id = chat.Id,
                AdminId = chat.Admin.Id,
                Name = chat.Name,
                MemberIds = chat.Members.Select(m => m.Id)
                                        .ToArray(),
                ChatCover = coverPics,
                UnreadCount = unreadCount,
                IsDialogue = chat.IsDialogue,
            };
        }
        public async Task<Chat[]> Convert(DBChat[] chats, int[] unreadCounts)
        {
            var covers = await Files.GetImageSetWithoutData(chats.Select(u => u.CoverImageId).ToArray());
            var result = new Chat[chats.Length];
            for (int i = 0; i < chats.Length; i++)
                result[i] = Convert(chats[i], unreadCounts[i], covers[i]);
            return result;
        }

        public async Task<Message> Convert(DBMessage dbMessage, bool isReaded)
        {
            if (dbMessage.IsSystem && dbMessage.Action != null)
            {
                return new Message(dbMessage.Id, dbMessage.AuthorId, dbMessage.DestinationId,
                                 dbMessage.Content, dbMessage.SendTime, isReaded, dbMessage.Action.Kind,
                                 dbMessage.Action.ExecutorId, dbMessage.Action.TargetId == null ? -1 : dbMessage.Action.TargetId.Value);
            }

            var (images, files) = await GetMessageAttachments(dbMessage.Attachments.ToArray());
            return new Message(dbMessage.Id, dbMessage.AuthorId, dbMessage.DestinationId,
                               dbMessage.Content, dbMessage.SendTime, isReaded, images, files);
        }

        public async Task<Message[]> Convert(DBMessage[] dbMessages, bool[] isReaded)
        {
            var result = new Message[dbMessages.Length];
            for (int i = 0; i < dbMessages.Length; i++)
                result[i] = await Convert(dbMessages[i], isReaded[i]);

            return result;
        }

        public async Task<User> Convert(DBUser user)
        {
            return Convert(user, await Files.GetImageSetWithoutData(user.ProfileImageId));
        }
        public User Convert(DBUser dBUser, DBImageSet? profilePic)
        {
            PhotoLink[] pics = [];
            if (profilePic != null)
                pics = Convert(profilePic);

            return new User()
            {
                Name = dBUser.Name,
                Id = dBUser.Id,
                Login = dBUser.Login,
                IsOnline = dBUser.IsOnline,
                LastAction = dBUser.LastRequest,
                ProfilePics = pics,
            };
        }
        public async Task<User[]> Convert(DBUser[] users)
        {
            var profilePics = await Files.GetImageSetWithoutData(users.Select(u => u.ProfileImageId)
                                                                     .ToArray());
            var result = new User[users.Length];
            for (int i = 0; i < users.Length; i++)
                result[i] = Convert(users[i], profilePics[i]);
            return result;
        }

        public ChatInvite Convert(DBChatInvite inivite)
        {
            return new ChatInvite(inivite.ChatId,
                                  inivite.ToUserId,
                                  inivite.InviterId,
                                  inivite.Id);
        }
        public ChatInvite[] Convert(DBChatInvite[] invites)
        {
            var result = new ChatInvite[invites.Length];
            for (int i = 0; i < invites.Length; i++)
                result[i] = Convert(invites[i]);
            return result;
        }

        public FriendRequest Convert(DBFriendRequest request)
        {
            return new FriendRequest(request.ReceiverId, request.SenderId, request.Id);
        }
        public FriendRequest[] Convert(DBFriendRequest[] requests)
        {
            var result = new FriendRequest[requests.Length];
            for (int i = 0; i < requests.Length; i++)
                result[i] = Convert(requests[i]);
            return result;
        }

        public PhotoLink[] Convert(DBImageSet set)
        {
            return Convert(set.Images.ToArray());
        }
        public PhotoLink[] Convert(DBImage[] images)
        {
            var result = new PhotoLink[images.Length];
            for (int i = 0; i < result.Length; i++)
            {
                var url = $"images/{images[i].Url}/{images[i].Id}";
                var size = Convert(images[i].Size);
                result[i] = new PhotoLink(url, size);
            }
            return result;
        }
        public FileLink[] Convert(DBBinaryFile[] files)
        {
            var result = new FileLink[files.Length];
            for (int i = 0; i < result.Length; i++)
            {
                var url = $"files/{files[i].Url}/{files[i].Id}";
                var name = files[i].Name;
                result[i] = new FileLink(url, name);
            }
            return result;
        }
        private ApiImageSize Convert(DBImageSize imageSize)
        {
            return imageSize switch
            {
                DBImageSize.Small => ApiImageSize.Small,
                DBImageSize.Medium => ApiImageSize.Medium,
                DBImageSize.Large => ApiImageSize.Large,
                _ => throw new NotImplementedException(),
            };
        }

        private async Task<(PhotoLink[], FileLink[])> GetMessageAttachments(DBMessageAttachments[] attachments)
        {
            var images = new List<int>();
            var files = new List<int>();

            foreach (var attachment in attachments)
            {
                switch (attachment.Kind)
                {
                    case AttachmentKind.Image:
                        images.Add(attachment.AttachmentId);
                        break;
                    case AttachmentKind.File:
                        files.Add(attachment.AttachmentId);
                        break;
                }
            }
            return (Convert(await Files.GetImageWithoutData(images)),
                    Convert(await Files.GetFilesWithoutData(files)));
        }
    }
}
