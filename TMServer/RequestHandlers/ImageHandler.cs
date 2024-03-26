using ApiTypes;
using ApiTypes.Communication.Medias;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Interaction;

namespace TMServer.RequestHandlers
{
    internal static class ImageHandler
    {
        public static void SetProfileImage(ApiData<ChangeUserProfileImageRequest> request)
        {
            var image = IsValideProfileImage(request.Data.ImageData);
            if (image == null)
                return;

            var set = Images.AddImageAsSet(image);
            if (set == null)
                return;

            Users.SetProfileImage(request.UserId, set.Id);
        }

        public static byte[] GetImage(string url, int id)
        {
            var image = Images.GetImage(id);

            if (image == null || image.Url != url)
                return [];
            return image.Data;
        }

        private static Image? IsValideProfileImage(byte[] imageData)
        {
            try
            {
                return Image.Load(imageData);
            }
            catch
            {
                return null;
            }
        }

    }
}
