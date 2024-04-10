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
using TMServer.DataBase.Tables.FileTables;

namespace TMServer.RequestHandlers
{
    public class FileHandler
    {
        private readonly Files Files;
        private readonly Chats Chats;
        private readonly Users Users;
        private readonly Messages Messages;
        private readonly DbConverter Converter;
        private readonly Security Security;

        public FileHandler(Files files, Chats chats, Users users, Messages messages, Security security, DbConverter converter)
        {
            Files = files;
            Chats = chats;
            Users = users;
            Messages = messages;
            Converter = converter;
            Security = security;
        }
        public User? SetProfileImage(ApiData<ChangeProfileImageRequest> request)
        {
            using var image = IsValidImage(request.Data.ImageData,true);
            if (image == null)
                return null;

            var set = Files.AddImageAsSet(image);
            if (set == null)
                return null;

            var user = Users.SetProfileImage(request.UserId, set.Id, out var prevId);

            if (user == null)
                return null;

            if (prevId > 0)
                Files.RemoveSet(prevId);

            return Converter.Convert(user, set);
        }

        public async Task<byte[]> GetImageAsync(string url, int id)
        {
            var image = await Files.GetImageWithDataAsync(id);

            if (image == null || image.Url != url)
                return [];
            return image.Data;
        }

        public async Task<DBBinaryFile?> GetFileAsync(string url, int id)
        {
            var file = await Files.GetFileWithDataAsync(id);

            if (file == null || file.Url != url)
                return null;
            return file;
        }


        private Image? IsValidImage(byte[] imageData, bool isProfile)
        {
            try
            {
                var image = Image.Load(imageData);
                if ((isProfile && Security.IsValidProfileImage(image)) ||
                    (!isProfile && Security.IsValidImage(image)))
                    return image;

                image.Dispose();
                return null;
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

            using var image = IsValidImage(request.Data.NewCover,true);
            if (image == null)
                return;

            var set = Files.AddImageAsSet(image);
            if (set == null)
                return;

            var chat = Chats.SetCover(request.UserId, request.Data.ChatId, set.Id, out var prevId);
            if (chat == null)
                return;

            if (prevId > 0)
                Files.RemoveSet(prevId);
        }

        internal Message? MessageWithFiles(ApiData<MessageWithFilesSendRequest> request)
        {
            if (!Security.IsMessageWithFilesLegal(request.UserId,request.Data))
                return null;

            var images = new List<Image>();
            var files = new List<SerializableFile>();
            foreach (var file in request.Data.Files)
            {
                if (IsHaveImageExtension(file))
                {
                    var image = IsValidImage(file.Data, false);
                    if (image != null)
                    {
                        images.Add(image);
                        continue;
                    }
                }
                files.Add(file);
            }

            var dbImages = Files.AddImages(images.ToArray());
            var dbFiles = Files.AddFiles(files.ToArray());

            var dbMessage = Messages.AddMessage(request.UserId, request.Data.Text, dbImages, dbFiles, request.Data.DestinationId);
            Messages.AddToUnread(dbMessage.Id, dbMessage.DestinationId);
            var isReaded = Messages.IsMessageReaded(request.UserId, dbMessage.Id);

            Messages.ReadAllInChat(request.UserId, request.Data.DestinationId);
            return Converter.Convert(dbMessage, isReaded);
        }
    }
}
