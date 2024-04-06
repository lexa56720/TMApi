﻿using System;
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
    public class Images
    {
        public DBImage AddImage(Image largeImage)
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
        public DBImageSet? AddImageAsSet(Image largeImage)
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



        public DBImageSet?[] GetImageSet(int[] setIds)
        {
            using var db = new ImagesDBContext();
            var set = db.ImageSets.Include(s => s.Images)
                                  .Where(s => setIds.Contains(s.Id))
                                  .ToArray();
            return setIds.Select(id => set.FirstOrDefault(i => i.Id == id))
                         .ToArray();
        }
        public DBImageSet? GetImageSet(int setId)
        {
            using var db = new ImagesDBContext();
            var set = db.ImageSets.Include(s => s.Images)
                                  .SingleOrDefault(i => i.Id == setId);
            return set;
        }

        public async Task<DBImage?> GetImageAsync(int imageId)
        {
            using var db = new ImagesDBContext();

            var image = await db.Images.SingleOrDefaultAsync(i => i.Id == imageId);

            return image;
        }
        public DBImage[] GetImage(int[] imageIds)
        {
            using var db = new ImagesDBContext();

            var images = db.Images.Where(i => imageIds.Contains(i.Id))
                                  .ToArray();

            return images;
        }

        public bool RemoveSet(int setId)
        {
            using var db = new ImagesDBContext();
            var set = db.ImageSets.Include(s => s.Images)
                                  .SingleOrDefault(i => i.Id == setId);
            if (set == null)
                return false;

            db.Images.RemoveRange(set.Images);
            db.ImageSets.Remove(set);
            return db.SaveChanges() > 0;
        }

        public string GetImageUrl(int id, ImageSize size)
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
