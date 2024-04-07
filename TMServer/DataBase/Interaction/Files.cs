using ApiTypes.Communication.BaseTypes;
using Microsoft.EntityFrameworkCore;
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
                    Name = files[i].Name,
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


        private string GenerateUrl()
        {
            return RandomNumberGenerator.GetHexString(128, true);
        }
    }
}
