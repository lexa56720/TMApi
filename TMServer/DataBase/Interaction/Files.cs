using ApiTypes.Communication.BaseTypes;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Tables.FileTables;

namespace TMServer.DataBase.Interaction
{
    public class Files
    {
        public DBBinaryFile[] AddFiles(SerializableFile[] files)
        {
            using var db = new FilesDBContext();

            var result = new DBBinaryFile[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                var dbFile = new DBBinaryFile()
                {
                    Url = GenerateUrl(),
                    Data = files[i].Data,
                    Name = CutName(files[i].Name),
                };
                db.Files.Add(dbFile);
                result[i] = dbFile;
            }
            db.SaveChanges();
            return result;
        }

        public DBBinaryFile[] GetFiles(IEnumerable<int> fileIds)
        {
            using var db = new FilesDBContext();

            var files = db.Files.Where(f => fileIds.Contains(f.Id))
                                .ToArray();

            return files;
        }

        public async Task<DBBinaryFile?> GetFileAsync(int fileId)
        {
            using var db = new FilesDBContext();

            var file = await db.Files.SingleOrDefaultAsync(f => f.Id == fileId);

            return file;
        }

        public DBImage[] AddImages(Image[] images)
        {
            using var db = new FilesDBContext();

            var result = new DBImage[images.Length];
            for (int i = 0; i < images.Length; i++)
            {
                var imageData = GetImageBytes(images[i]);
                var dbImage = new DBImage()
                {
                    Url = GenerateUrl(),
                    Data = imageData,
                    Size = ImageSize.Large
                };
                db.Images.Add(dbImage);
                result[i] = dbImage;
            }
            db.SaveChanges();
            return result;
        }
        public DBImageSet? AddImageAsSet(Image largeImage)
        {
            using var smallImage = largeImage.Clone(image => image.Resize(64, 64));
            using var mediumImage = largeImage.Clone(image => image.Resize(128, 128));
            largeImage.Mutate(image => image.Resize(256, 256));

            var smallImageData = GetImageBytes(smallImage);
            var mediumImageData = GetImageBytes(mediumImage);
            var largeImageData = GetImageBytes(largeImage);

            using var db = new FilesDBContext();

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
            using var db = new FilesDBContext();
            var set = db.ImageSets.Include(s => s.Images)
                                  .Where(s => setIds.Contains(s.Id))
                                  .ToArray();
            return setIds.Select(id => set.FirstOrDefault(i => i.Id == id))
                         .ToArray();
        }
        public DBImageSet? GetImageSet(int setId)
        {
            using var db = new FilesDBContext();
            var set = db.ImageSets.Include(s => s.Images)
                                  .SingleOrDefault(i => i.Id == setId);
            return set;
        }

        public async Task<DBImage?> GetImageAsync(int imageId)
        {
            using var db = new FilesDBContext();

            var image = await db.Images.SingleOrDefaultAsync(i => i.Id == imageId);

            return image;
        }
        public DBImage[] GetImage(IEnumerable<int> imageIds)
        {
            using var db = new FilesDBContext();

            var images = db.Images.Where(i => imageIds.Contains(i.Id))
                                  .ToArray();

            return images;
        }

        public bool RemoveSet(int setId)
        {
            using var db = new FilesDBContext();
            var set = db.ImageSets.Include(s => s.Images)
                                  .SingleOrDefault(i => i.Id == setId);
            if (set == null)
                return false;

            db.Images.RemoveRange(set.Images);
            db.ImageSets.Remove(set);
            return db.SaveChanges() > 0;
        }

        private byte[] GetImageBytes(Image image)
        {
            using var ms = new MemoryStream();
            image.SaveAsJpeg(ms);
            return ms.ToArray();
        }
        private string CutName(string name)
        {
            if (name.Length < 128)
                return name;
            return name.Substring(0, 128);
        }
        private string GenerateUrl()
        {
            return RandomNumberGenerator.GetHexString(128, true);
        }
    }
}
