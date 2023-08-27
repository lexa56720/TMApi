using ApiTypes.Shared;
using CSDTP.Cryptography.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Tables;
using TMServer.DataBase.Types;

namespace TMServer.DataBase
{
    internal static class Security
    {

        public static int SaveRsaKeyPair(string serverPrivateKey, string clientPublicKey)
        {
            using var db = new TmdbContext();

            var rsa = new DBRsa()
            {
                PrivateServerKey = serverPrivateKey,
                PublicClientKey = clientPublicKey,
                CreateDate = DateTime.UtcNow,
            };
            db.RsaCrypts.Add(rsa);
            db.SaveChanges();

            return rsa.Id;
        }

        public static DBRsa? GetRsaKeysById(int rsaId)
        {
            using var db = new TmdbContext();
            return db.RsaCrypts.Find(rsaId);
        }

        public static void SetDeprecated(int cryptId)
        {
            using var db = new TmdbContext();

            var aes = db.AesCrypts.Find(cryptId);

            if (aes != null)
            {
                aes.IsDeprecated = true;
                aes.DeprecatedDate = DateTime.UtcNow;
            }

            db.SaveChanges();
        }

        public static byte[] GetAesKey(int cryptId)
        {
            using var db = new TmdbContext();

            var dbAes = db.AesCrypts.SingleOrDefault(a => a.CryptId == cryptId);
            ArgumentNullException.ThrowIfNull(dbAes);

            return dbAes.AesKey;
        }

        public static bool IsTokenCorrect(string token, int userId)
        {
            using var db = new TmdbContext();
            var dbToken = db.Tokens.SingleOrDefault(t => t.UserId == userId);
            if (dbToken != null)
                return dbToken.AccessToken.Equals(token) && DateTime.UtcNow < dbToken.Expiration;
            return false;
        }
    }
}
