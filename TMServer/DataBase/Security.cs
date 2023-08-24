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

            var rsa = new RsaCrypt()
            {
                PrivateServerKey = serverPrivateKey,
                PublicClientKey = clientPublicKey,
                CreateDate = DateTime.UtcNow,
            };
            db.RsaCrypts.Add(rsa);
            db.SaveChanges();

            return rsa.Id;
        }

        public static RsaCrypt? GetRsaKeysById(int rsaId)
        {
            using var db = new TmdbContext();
            return db.RsaCrypts.Find(rsaId);
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

                var dbCrypt = new AesCrypt()
                {
                    AesKey = HashGenerator.BytesToString(aes.Key),
                    IV = HashGenerator.BytesToString(aes.IV),
                };
                db.AesCrypts.Add(dbCrypt);
                db.SaveChanges();

                db.Users.Add(new User()
                {
                    Login = login,
                    LastRequest = DateTime.UtcNow,
                    Name = login,
                    CryptId = dbCrypt.CryptId,
                    Password = password,
                });
                db.SaveChanges();

                return true;
            }
            return false;
        }

        public static int SaveAuth(int userId, byte[] aesKey, byte[] aesIV, string token, DateTime expiration)
        {
            AddOrUpdateToken(userId, token, expiration);
            return AddOrUpdateAes(userId, aesKey, aesIV);
        }

        private static int AddOrUpdateAes(int userId, byte[] aesKey, byte[] aesIV)
        {
            using var db = new TmdbContext();

            var exist = db.AesCrypts.SingleOrDefault(a => a.User.Id == userId);
            if (exist == null)
            {
                var aes = new AesCrypt()
                {
                    AesKey = HashGenerator.BytesToString(aesKey),
                    IV = HashGenerator.BytesToString(aesIV),
                };
                db.AesCrypts.Add(aes);
                db.SaveChanges();
                return aes.CryptId;
            }
            exist.AesKey = HashGenerator.BytesToString(aesKey);
            exist.IV = HashGenerator.BytesToString(aesIV);
            return exist.CryptId;
        }

        private static void AddOrUpdateToken(int userId, string token, DateTime expiration)
        {
            using var db = new TmdbContext();

            var dbToken = db.Tokens.SingleOrDefault(t => t.UserId == userId);
            if (dbToken != null)
            {
                dbToken.AccessToken = token;
                dbToken.Expiration = expiration;
            }
            else
            {
                db.Tokens.Add(new Token()
                {
                    AccessToken = token,
                    UserId = userId,
                    Expiration = expiration
                });
            }
            db.SaveChanges();
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
