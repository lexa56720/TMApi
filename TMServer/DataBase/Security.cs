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
        public static void SaveRsaKeyPair(uint ip, string serverPrivateKey, string clientPublicKey)
        {
            using var db = new TmdbContext();
            db.RsaCrypts.Update(new RsaCrypt
            {
                Ip = ip,
                PrivateServerKey = serverPrivateKey,
                PublicClientKey = clientPublicKey,
                CreateDate = DateTime.UtcNow,
            });
            db.SaveChanges();
        }

        public static RsaCrypt? GetRsaKeysByIp(uint ip)
        {
            using var db = new TmdbContext();
            return db.RsaCrypts.Find(ip);
        }

        public static AesCrypt? GetAesKeysByCryptId(int cryptId)
        {
            using var db = new TmdbContext();
            return db.AesCrypts.Find(cryptId);
        }

        public static int GetUserId(string login, string password)
        {
            using var db = new TmdbContext();
            var user = db.Users.First(u => u.Login == login && u.Password == password);
            return user == null ? -1 : user.Id;
        }

        public static bool IsLoginAvailable(string login)
        {
            using var db = new TmdbContext();
            return !db.Users.Any(u => u.Login == login);
        }

        public static bool CreateUser(string login, string password)
        {
            using var db = new TmdbContext();
            if (!db.Users.Any(u => u.Login == login))
            {
                var aes = new AesEncrypter();
                var cryptId = db.AesCrypts.Add(new AesCrypt()
                {
                    AesKey = HashGenerator.BytesToString(aes.Key),
                    IV = HashGenerator.BytesToString(aes.IV),
                }).Entity.CryptId;

                db.Users.Add(new User()
                {
                    Login = login,
                    LastRequest = DateTime.UtcNow,
                    Name = login,
                    CryptId = cryptId,
                    Password = password,
                });
                db.SaveChanges();

                return true;
            }
            return false;
        }

        public static int SaveAuth(int userId, byte[] aesKey, byte[] aesIV, string token, DateTime expiration)
        {
            using var db = new TmdbContext();

            var dbToken = db.Tokens.First(t => t.UserId == userId);

            dbToken.AccessToken = token;
            dbToken.Expiration = expiration;

            db.AesCrypts.Remove(db.AesCrypts.First(a => a.User.Id == userId));

            var cryptId = db.AesCrypts.Add(new AesCrypt()
            {
                AesKey = HashGenerator.BytesToString(aesKey),
                IV = HashGenerator.BytesToString(aesIV),
            }).Entity.CryptId;

            db.SaveChanges();
            return cryptId;
        }

        public static bool IsTokenCorrect(string token, int userId)
        {
            using var db = new TmdbContext();
            var dbToken = db.Tokens.First(t => t.UserId == userId);
            ArgumentNullException.ThrowIfNull(dbToken);
            return dbToken.AccessToken == token;
        }
        public static void UpdateAuth(int cryptId, byte[] aesIV)
        {
            using var db = new TmdbContext();

            var aes = db.AesCrypts.Find(cryptId);

            aes.IV = HashGenerator.BytesToString(aesIV);

            db.SaveChanges();
        }

        public static AesEncrypter GetAuth(int cryptId)
        {
            using var db = new TmdbContext();

            var dbAes = db.AesCrypts.Find(cryptId);
            ArgumentNullException.ThrowIfNull(dbAes);

            return new AesEncrypter(
                HashGenerator.Base64ToBytes(dbAes.AesKey), 
                HashGenerator.Base64ToBytes(dbAes.IV));
        }
    }
}
