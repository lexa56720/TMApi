using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase.Interaction
{
    public class Crypt
    {
        public async Task<int> SaveRsaKeyPair(string serverPrivateKey, string clientPublicKey, DateTime expiration)
        {
            using var db = new TmdbContext();

            var rsa = new DBRsa()
            {
                PrivateServerKey = serverPrivateKey,
                PublicClientKey = clientPublicKey,
                Expiration = expiration,
            };
            await db.RsaCrypts.AddAsync(rsa);
            await db.SaveChangesAsync();

            return rsa.Id;
        }

        public async Task<DBRsa?> GetRsaKeysById(int rsaId)
        {
            using var db = new TmdbContext();
            var now = DateTime.UtcNow;
            return await db.RsaCrypts.SingleOrDefaultAsync(rsa => rsa.Id == rsaId && rsa.Expiration > now);
        }

        public async Task SetDeprecated(int cryptId)
        {
            using var db = new TmdbContext();

            var aes = await db.AesCrypts.FindAsync(cryptId);

            if (aes != null)
                aes.Expiration = DateTime.UtcNow + TimeSpan.FromHours(1);

            await db.SaveChangesAsync();
        }

        public async Task<byte[]?> GetAesKey(int cryptId)
        {
            using var db = new TmdbContext();

            var dbAes = await db.AesCrypts.SingleOrDefaultAsync(a =>
            a.Id == cryptId && a.Expiration > DateTime.UtcNow);
            if (dbAes == null)
                return null;

            return dbAes.AesKey;
        }
    }
}
