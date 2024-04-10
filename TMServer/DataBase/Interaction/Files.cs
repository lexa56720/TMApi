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
        private readonly string FilesFolderPath;
        private readonly string ImagesFolderPath;

        public Files(string filesFolderPath, string imagesFolderPath)
        {
            FilesFolderPath = filesFolderPath;
            ImagesFolderPath = imagesFolderPath;

            if(!Directory.Exists(filesFolderPath))
                Directory.CreateDirectory(filesFolderPath);

            if (!Directory.Exists(imagesFolderPath))
                Directory.CreateDirectory(imagesFolderPath);
        }

        public DBBinaryFile[] AddFiles(SerializableFile[] files)
        {
            using var db = new FilesDBContext();

            var result = new DBBinaryFile[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                var dbFile = new DBBinaryFile()
                {
                    Url = GenerateUrl(),
                    Name = CutName(files[i].Name),
                };
                db.Files.Add(dbFile);
                result[i] = dbFile;
            }
            db.SaveChanges();
            for (int i = 0; i < files.Length; i++)
                SaveBinaryFileData(result[i], files[i].Data);
            return result;
        }
        public DBBinaryFile[] GetFilesWithoutData(IEnumerable<int> fileIds)
        {
            using var db = new FilesDBContext();

            var files = db.Files.Where(f => fileIds.Contains(f.Id))
                                .ToArray();

            return files;
        }
        public async Task<DBBinaryFile?> GetFileWithDataAsync(int fileId)
        {
            using var db = new FilesDBContext();

            var file = await db.Files.SingleOrDefaultAsync(f => f.Id == fileId);
            if (file != null)
                file.Data = await GetBinaryFileDataAsync(file);

            return file;
        }

        public DBImage[] AddImages(Image[] images)
        {
            using var db = new FilesDBContext();

            var result = new DBImage[images.Length];
            for (int i = 0; i < images.Length; i++)
            {

                var dbImage = new DBImage()
                {
                    Url = GenerateUrl(),
                    Size = ImageSize.Large
                };
                db.Images.Add(dbImage);
                result[i] = dbImage;

            }
            db.SaveChanges();
            for (int i = 0; i < images.Length; i++)
                SaveImageData(result[i], ImageToBytes(images[i]));
            return result;
        }
        public DBImageSet? AddImageAsSet(Image largeImage)
        {
            using var smallImage = largeImage.Clone(image => image.Resize(64, 64));
            using var mediumImage = largeImage.Clone(image => image.Resize(128, 128));
            largeImage.Mutate(image => image.Resize(256, 256));

            var smallImageData = ImageToBytes(smallImage);
            var mediumImageData = ImageToBytes(mediumImage);
            var largeImageData = ImageToBytes(largeImage);

            using var db = new FilesDBContext();
            var set = new DBImageSet();
            var smallDbImage = AddSetPart(smallImage, set, ImageSize.Small, db);
            var mediumDbImage = AddSetPart(mediumImage, set, ImageSize.Medium, db);
            var largeDbImage = AddSetPart(largeImage, set, ImageSize.Large, db);
            db.SaveChanges();

            SaveImageData(smallDbImage, smallImageData);
            SaveImageData(mediumDbImage, mediumImageData);
            SaveImageData(largeDbImage, largeImageData);

            return set;
        }
        private DBImage AddSetPart(Image image, DBImageSet set, ImageSize size, FilesDBContext db)
        {
            var imageData = ImageToBytes(image);
            var dbImage = new DBImage()
            {
                Set = set,
                Url = GenerateUrl(),
                Size = size
            };
            db.Images.Add(dbImage);
            return dbImage;
        }

        public DBImageSet?[] GetImageSetWithoutData(int[] setIds)
        {
            using var db = new FilesDBContext();
            var set = db.ImageSets.Include(s => s.Images)
                                  .Where(s => setIds.Contains(s.Id))
                                  .ToArray();
            return setIds.Select(id => set.FirstOrDefault(i => i.Id == id))
                         .ToArray();
        }
        public DBImageSet? GetImageSetWithoutData(int setId)
        {
            using var db = new FilesDBContext();
            var set = db.ImageSets.Include(s => s.Images)
                                  .SingleOrDefault(i => i.Id == setId);
            return set;
        }
        public DBImage[] GetImageWithoutData(IEnumerable<int> imageIds)
        {
            using var db = new FilesDBContext();

            var images = db.Images.Where(i => imageIds.Contains(i.Id))
                                  .ToArray();
            return images;
        }

        public async Task<DBImage?> GetImageWithDataAsync(int imageId)
        {
            using var db = new FilesDBContext();

            var image = await db.Images.SingleOrDefaultAsync(i => i.Id == imageId);
            if (image != null)
                image.Data = await GetImageDataAsync(image);
            return image;
        }

        public bool RemoveSet(int setId)
        {
            using var db = new FilesDBContext();
            var set = db.ImageSets.Include(s => s.Images)
                                  .SingleOrDefault(i => i.Id == setId);
            if (set == null)
                return false;

            foreach (var image in set.Images)
                RemoveImageData(image);

            db.Images.RemoveRange(set.Images);
            db.ImageSets.Remove(set);
            return db.SaveChanges() > 0;
        }
        private byte[] ImageToBytes(Image image)
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

        private string GetImagePath(DBImage image)
        {
            return Path.Combine(ImagesFolderPath, image.Id.ToString());
        }
        private string GetBinaryFilePath(DBBinaryFile file)
        {
            return Path.Combine(FilesFolderPath, file.Id.ToString());
        }

        private void SaveImageData(DBImage image, byte[] data)
        {
            SaveData(GetImagePath(image), data);
        }
        private async Task<byte[]> GetImageDataAsync(DBImage image)
        {
            return await GetDataAsync(GetImagePath(image));
        }

        private void SaveBinaryFileData(DBBinaryFile file, byte[] data)
        {
            SaveData(GetBinaryFilePath(file), data);
        }
        private async Task<byte[]> GetBinaryFileDataAsync(DBBinaryFile file)
        {
            return await GetDataAsync(GetBinaryFilePath(file));
        }

        private void SaveData(string path, byte[] data)
        {
            System.IO.File.WriteAllBytes(path, data);
        }
        private async Task<byte[]> GetDataAsync(string path)
        {
            return await System.IO.File.ReadAllBytesAsync(path);
        }

        private void RemoveImageData(DBImage image)
        {
            try
            {
                var path = GetImagePath(image);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }
            catch
            {
            }
        }
    }
}
