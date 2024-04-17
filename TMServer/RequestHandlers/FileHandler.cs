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
        public async Task<User?> SetProfileImage(ApiData<ChangeProfileImageRequest> request)
        {
            using var image = IsValidImage(request.Data.ImageData, true);
            if (image == null)
                return null;

            var set = await Files.AddImageAsSet(image);
            if (set == null)
                return null;

            var (user, prevId) = await Users.SetProfileImage(request.UserId, set.Id);

            if (user == null)
                return null;

            if (prevId > 0)
                await Files.RemoveSet(prevId);

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

        internal async Task SetChatCover(ApiData<ChagneCoverRequest> request)
        {
            if (!await Security.IsAdminOfChat(request.UserId, request.Data.ChatId))
                return;

            using var image = IsValidImage(request.Data.NewCover, true);
            if (image == null)
                return;

            var set = await Files.AddImageAsSet(image);
            if (set == null)
                return;

            var (chat, prevId) = await Chats.SetCover(request.UserId, request.Data.ChatId, set.Id);
            if (chat == null)
                return;

            if (prevId > 0)
                await Files.RemoveSet(prevId);
        }

        internal async Task<Message?> MessageWithFiles(ApiData<MessageWithFilesSendRequest> request)
        {
            if (!await Security.IsMessageWithFilesLegal(request.UserId, request.Data))
                return null;

            var images = new List<Image>();
            var files = new List<SerializableFile>();
            foreach (var file in request.Data.Files)
            {
                if (IsHaveImageExtension(file))
                {
                    var image = IsValidImage(file.Data, false);
                    if (image == null)
                        continue;
                    images.Add(image);
                }
                else
                    files.Add(file);
            }

            var dbImages = await Files.AddImages(images.ToArray());
            var dbFiles = await Files.AddFiles(files.ToArray());

            var dbMessage = await Messages.AddMessage(request.UserId, request.Data.Text, dbImages, dbFiles, request.Data.DestinationId);
            await Messages.AddToUnread(dbMessage.Id, dbMessage.DestinationId);
            var isReaded = await Messages.IsMessageReaded(request.UserId, dbMessage.Id);

            await Messages.ReadAllInChat(request.UserId, request.Data.DestinationId);
            return await Converter.Convert(dbMessage, isReaded);
        }
    }
}
