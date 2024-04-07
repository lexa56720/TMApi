using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Chats;
using ApiTypes.Communication.Medias;
using ApiTypes.Communication.Messages;
using ApiTypes.Communication.Users;
using ApiTypes.Shared;
using CSDTP.Requests;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Interaction;

namespace TMServer.RequestHandlers
{
    public class FileHandler
    {
        private readonly Images Images;
        private readonly Files Files;
        private readonly Chats Chats;
        private readonly Users Users;
        private readonly Messages Messages;
        private readonly DbConverter Converter;
        private readonly Security Security;

        public FileHandler(Images images, Files files, Chats chats, Users users, Messages messages, Security security, DbConverter converter)
        {
            Images = images;
            Files = files;
            Chats = chats;
            Users = users;
            Messages = messages;
            Converter = converter;
            Security = security;
        }
        public User? SetProfileImage(ApiData<ChangeProfileImageRequest> request)
        {
            using var image = IsValidImage(request.Data.ImageData);
            if (image == null)
                return null;

            var set = Images.AddImageAsSet(image);
            if (set == null)
                return null;

            var user = Users.SetProfileImage(request.UserId, set.Id, out var prevId);

            if (user == null)
                return null;

            if (prevId > 0)
                Images.RemoveSet(prevId);

            return Converter.Convert(user, set);
        }

        public async Task<byte[]> GetImageAsync(string url, int id)
        {
            var image = await Images.GetImageAsync(id);

            if (image == null || image.Url != url)
                return [];
            return image.Data;
        }

        public async Task<byte[]> GetFileAsync(string url, int id)
        {
            var file = await Files.GetFileAsync(id);

            if (file == null || file.Url != url)
                return [];
            return file.Data;
        }
        private Image? IsValidImage(byte[] imageData)
        {
            try
            {
                var image = Image.Load(imageData);
                if (image.Width < 64 || image.Height < 64)
                {
                    image.Dispose();
                    return null;
                }
                return image;
            }
            catch
            {
                return null;
            }
        }
        private bool IsHaveImageExtension(SerializableFile file)
        {
            var ext = file.Name.Split('.', StringSplitOptions.TrimEntries).LastOrDefault();
            return ext != null && (ext == "png" || ext == "jpg" || ext == "jpeg");
        }

        internal void SetChatCover(ApiData<ChagneCoverRequest> request)
        {
            if (!Security.IsAdminOfChat(request.UserId, request.Data.ChatId))
                return;

            using var image = IsValidImage(request.Data.NewCover);
            if (image == null)
                return;

            var set = Images.AddImageAsSet(image);
            if (set == null)
                return;

            var chat = Chats.SetCover(request.UserId, request.Data.ChatId, set.Id, out var prevId);
            if (chat == null)
                return;

            if (prevId > 0)
                Images.RemoveSet(prevId);
        }

        internal Message? MessageWithFiles(ApiData<MessageWithFilesSendRequest> request)
        {
            if (!DataConstraints.IsMessageLegal(request.Data.Text) ||
                !Security.IsMemberOfChat(request.UserId, request.Data.DestinationId))
                return null;

            List<Image> images = new List<Image>();
            List<SerializableFile> files = new List<SerializableFile>();
            foreach (var file in request.Data.Files)
            {
                if (IsHaveImageExtension(file))
                {
                    var image = IsValidImage(file.Data);
                    if (image != null)
                    {
                        images.Add(image);
                        continue;
                    }
                }
                files.Add(file);
            }

            var dbImages = Images.AddImages(images.ToArray());
            var dbFiles = Files.AddFiles(files.ToArray());

            var dbMessage = Messages.AddMessage(request.UserId, request.Data.Text, dbImages, dbFiles, request.Data.DestinationId);
            Messages.AddToUnread(dbMessage.Id, dbMessage.DestinationId);
            var isReaded = Messages.IsMessageReaded(request.UserId, dbMessage.Id);

            Messages.ReadAllInChat(request.UserId, request.Data.DestinationId);
            return Converter.Convert(dbMessage, isReaded);
        }
    }
}
