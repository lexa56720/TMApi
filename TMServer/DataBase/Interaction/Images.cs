using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using TMServer.DataBase.Tables;


namespace TMServer.DataBase.Interaction
{
    internal class Images
    {
        public static DBImage AddImage(Image largeImage)
        {
            var largeImageData = GetImageBytes(largeImage);
            using var db = new ImagesDBContext();

            var image = new DBImage()
            {
                Url = GenerateUrl(),
                Data = largeImageData,
                Size = ImageSize.Large
            };
            db.Images.Add(image);
            db.SaveChanges();
            return image;
        }
        public static DBImageSet? AddImageAsSet(Image largeImage)
        {
            using var smallImage = largeImage.Clone(image => image.Resize(64, 64));
            using var mediumImage = largeImage.Clone(image => image.Resize(128, 128));
            largeImage.Mutate(image => image.Resize(256, 256));

            var smallImageData = GetImageBytes(smallImage);
            var mediumImageData = GetImageBytes(mediumImage);
            var largeImageData = GetImageBytes(largeImage);

            using var db = new ImagesDBContext();

            var set = new DBImageSet();
            db.ImageSets.Add(set);
            db.Images.Add(new DBImage()
            {
                Set = set,
                Url = GenerateUrl(),
                Data = largeImageData,
                Size = ImageSize.Large
            });
            db.Images.Add(new DBImage()
            {
                Set = set,
                Url = GenerateUrl(),
                Data = mediumImageData,
                Size = ImageSize.Medium
            });
            db.Images.Add(new DBImage()
            {
                Set = set,
                Url = GenerateUrl(),
                Data = smallImageData,
                Size = ImageSize.Small
            });

            db.SaveChanges();
            return set;
        }



        public static DBImageSet?[] GetImageSet(int[] setIds)
        {
            using var db = new ImagesDBContext();
            var set = db.ImageSets.Include(s => s.Images)
                                  .Where(s => setIds.Contains(s.Id))
                                  .ToArray();
            return setIds.Select(id => set.FirstOrDefault(i => i.Id == id))
                         .ToArray();
        }
        public static DBImageSet? GetImageSet(int setId)
        {
            using var db = new ImagesDBContext();
            var set = db.ImageSets.Include(s => s.Images)
                                  .SingleOrDefault(i => i.Id == setId);
            return set;
        }

        public static DBImage? GetImage(int imageId)
        {
            using var db = new ImagesDBContext();

            var image = db.Images.SingleOrDefault(i => i.Id == imageId);

            return image;
        }
        public static DBImage[] GetImage(int[] imageIds)
        {
            using var db = new ImagesDBContext();

            var images = db.Images.Where(i => imageIds.Contains(i.Id))
                                  .ToArray();

            return images;
        }

        public static string GetImageUrl(int id, ImageSize size)
        {
            using var db = new ImagesDBContext();

            var set = db.ImageSets.Include(s => s.Images)
                                  .SingleOrDefault(i => i.Id == id);
            if (set == null)
                return string.Empty;


            var image = set.Images.SingleOrDefault(i => i.Size == size);

            if (image == null)
                return string.Empty;

            return image.Url;
        }

        private static string GenerateUrl()
        {
            return RandomNumberGenerator.GetHexString(128, true);
        }

        private static byte[] GetImageBytes(Image image)
        {
            using var ms = new MemoryStream();
            image.SaveAsJpeg(ms);
            return ms.ToArray();
        }
    }
}
