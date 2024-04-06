using ApiTypes;
using ApiTypes.Communication.Chats;
using ApiTypes.Communication.Medias;
using ApiTypes.Communication.Users;
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
    public class ImageHandler
    {
        private readonly Images Images;
        private readonly Chats Chats;
        private readonly Users Users;
        private readonly DbConverter Converter;
        private readonly Security Security;

        public ImageHandler(Images images, Chats chats, Users users, Security security, DbConverter converter)
        {
            Images = images;
            Chats = chats;
            Users = users;
            Converter = converter;
            Security = security;
        }
        public User? SetProfileImage(ApiData<ChangeProfileImageRequest> request)
        {
            using var image = IsValideImage(request.Data.ImageData);
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

        private Image? IsValideImage(byte[] imageData)
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

        internal void SetChatCover(ApiData<ChagneCoverRequest> request)
        {
            if (!Security.IsAdminOfChat(request.UserId, request.Data.ChatId))
                return;

            using var image = IsValideImage(request.Data.NewCover);
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
    }
}
