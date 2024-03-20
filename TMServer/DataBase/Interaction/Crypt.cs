using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase.Interaction
{
    internal static class Crypt
    {
        public static int SaveRsaKeyPair(string serverPrivateKey, string clientPublicKey, DateTime expiration)
        {
            using var db = new TmdbContext();

            var rsa = new DBRsa()
            {
                PrivateServerKey = serverPrivateKey,
                PublicClientKey = clientPublicKey,
                Expiration = expiration,
            };
            db.RsaCrypts.Add(rsa);
            db.SaveChanges();

            return rsa.Id;
        }

        public static DBRsa? GetRsaKeysById(int rsaId)
        {
            using var db = new TmdbContext();
            var now = DateTime.UtcNow;
            return db.RsaCrypts.SingleOrDefault(rsa => rsa.Id == rsaId && rsa.Expiration > now);
        }

        public static void SetDeprecated(int cryptId)
        {
            using var db = new TmdbContext();

            var aes = db.AesCrypts.Find(cryptId);

            if (aes != null)
                aes.Expiration = DateTime.UtcNow+TimeSpan.FromHours(1);

            db.SaveChanges();
        }

        public static byte[]? GetAesKey(int cryptId)
        {
            using var db = new TmdbContext();

            var dbAes = db.AesCrypts.SingleOrDefault(a =>
            a.Id == cryptId && a.Expiration>DateTime.UtcNow);
            if (dbAes == null)
                return null;

            return dbAes.AesKey;
        }
    }
}
