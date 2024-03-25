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
        public DBImageSet AddImage(byte[] imageData)
        {
            using var largeImage = Image.Load(imageData);

            using var smallImage = largeImage.Clone(image => image.Resize(128, 128));
            using var mediumImage = largeImage.Clone(image => image.Resize(512, 512));
            largeImage.Mutate(image => image.Resize(1024, 1024));


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

        public DBImage GetImage(string url, int id)
        {
            using var db = new ImagesDBContext();

            return db.ImageSets.Include(s => s.Images)
                               .Single(i => i.Id == id)
                               .Images.Single(i => i.Url==url);
        }

        public string GetImageUrl(int id, ImageSize size)
        {
            using var db = new ImagesDBContext();

            return db.ImageSets.Include(s => s.Images)
                               .Single(i => i.Id == id)
                               .Images.Single(i => i.Size == size)
                                      .Url;
        }

        private string GenerateUrl()
        {
            return RandomNumberGenerator.GetHexString(128, true);
        }

        private byte[] GetImageBytes(Image image)
        {
            using var ms = new MemoryStream();
            image.SaveAsJpeg(ms);
            return ms.ToArray();
        }
    }
}
