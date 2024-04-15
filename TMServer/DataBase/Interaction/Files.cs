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

            if (!Directory.Exists(filesFolderPath))
                Directory.CreateDirectory(filesFolderPath);

            if (!Directory.Exists(imagesFolderPath))
                Directory.CreateDirectory(imagesFolderPath);
        }

        public async Task<DBBinaryFile[]> AddFiles(SerializableFile[] files)
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
                await db.Files.AddAsync(dbFile);
                result[i] = dbFile;
            }
            await db.SaveChangesAsync();

            var tasks = new Task[files.Length];
            for (int i = 0; i < files.Length; i++)
                tasks[i] = SaveBinaryFileData(result[i], files[i].Data);
            await Task.WhenAll(tasks);
            return result;
        }
        public async Task<DBBinaryFile[]> GetFilesWithoutData(IEnumerable<int> fileIds)
        {
            using var db = new FilesDBContext();

            return await db.Files.Where(f => fileIds.Contains(f.Id))
                                 .ToArrayAsync();
        }
        public async Task<DBBinaryFile?> GetFileWithDataAsync(int fileId)
        {
            using var db = new FilesDBContext();

            var file = await db.Files.SingleOrDefaultAsync(f => f.Id == fileId);
            if (file != null)
                file.Data = await GetBinaryFileDataAsync(file);
            return file;
        }

        public async Task<DBImage[]> AddImages(Image[] images)
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
                await db.Images.AddAsync(dbImage);
                result[i] = dbImage;

            }
            await db.SaveChangesAsync();
            var tasks = new Task[images.Length];
            for (int i = 0; i < images.Length; i++)
                tasks[i] = SaveImageDataAsync(result[i], await ImageToBytes(images[i]));
            await Task.WhenAll(tasks);
            return result;
        }
        public async Task<DBImageSet?> AddImageAsSet(Image largeImage)
        {
            using var smallImage = largeImage.Clone(image => image.Resize(64, 64));
            using var mediumImage = largeImage.Clone(image => image.Resize(128, 128));
            largeImage.Mutate(image => image.Resize(256, 256));

            var smallImageData = await ImageToBytes(smallImage);
            var mediumImageData = await ImageToBytes(mediumImage);
            var largeImageData = await ImageToBytes(largeImage);

            using var db = new FilesDBContext();
            var set = new DBImageSet();
            var smallDbImage = await AddSetPart(set, ImageSize.Small, db);
            var mediumDbImage = await AddSetPart(set, ImageSize.Medium, db);
            var largeDbImage = await AddSetPart(set, ImageSize.Large, db);
            await db.SaveChangesAsync();

            await SaveImageDataAsync(smallDbImage, smallImageData);
            await SaveImageDataAsync(mediumDbImage, mediumImageData);
            await SaveImageDataAsync(largeDbImage, largeImageData);

            return set;
        }
        private async Task<DBImage> AddSetPart(DBImageSet set, ImageSize size, FilesDBContext db)
        {
            var dbImage = new DBImage()
            {
                Set = set,
                Url = GenerateUrl(),
                Size = size
            };
            await db.Images.AddAsync(dbImage);
            return dbImage;
        }

        public async Task<DBImageSet?[]> GetImageSetWithoutData(int[] setIds)
        {
            using var db = new FilesDBContext();
            var set = await db.ImageSets.Include(s => s.Images)
                                        .Where(s => setIds.Contains(s.Id))
                                        .ToArrayAsync();
            return setIds.Select(id => set.FirstOrDefault(i => i.Id == id))
                         .ToArray();
        }
        public async Task<DBImageSet?> GetImageSetWithoutData(int setId)
        {
            using var db = new FilesDBContext();
            var set =await db.ImageSets.Include(s => s.Images)
                                       .SingleOrDefaultAsync(i => i.Id == setId);
            return set;
        }
        public async Task<DBImage[]> GetImageWithoutData(IEnumerable<int> imageIds)
        {
            using var db = new FilesDBContext();

            return await db.Images.Where(i => imageIds.Contains(i.Id))
                                  .ToArrayAsync();
        }

        public async Task<DBImage?> GetImageWithDataAsync(int imageId)
        {
            using var db = new FilesDBContext();

            var image = await db.Images.SingleOrDefaultAsync(i => i.Id == imageId);
            if (image != null)
                image.Data = await GetImageDataAsync(image);
            return image;
        }

        public async Task<bool> RemoveSet(int setId)
        {
            using var db = new FilesDBContext();
            var set =await db.ImageSets.Include(s => s.Images)
                                       .SingleOrDefaultAsync(i => i.Id == setId);
            if (set == null)
                return false;

            foreach (var image in set.Images)
                RemoveImageData(image);

            db.Images.RemoveRange(set.Images);
            db.ImageSets.Remove(set);
            return await db.SaveChangesAsync() > 0;
        }
        private async Task<byte[]> ImageToBytes(Image image)
        {
            using var ms = new MemoryStream();
            await image.SaveAsJpegAsync(ms);
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

        private async Task SaveImageDataAsync(DBImage image, byte[] data)
        {
            await SaveDataAsync(GetImagePath(image), data);
        }
        private async Task<byte[]> GetImageDataAsync(DBImage image)
        {
            return await GetDataAsync(GetImagePath(image));
        }

        private async Task SaveBinaryFileData(DBBinaryFile file, byte[] data)
        {
            await SaveDataAsync(GetBinaryFilePath(file), data);
        }
        private async Task<byte[]> GetBinaryFileDataAsync(DBBinaryFile file)
        {
            return await GetDataAsync(GetBinaryFilePath(file));
        }

        private async Task SaveDataAsync(string path, byte[] data)
        {
            await System.IO.File.WriteAllBytesAsync(path, data);
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
