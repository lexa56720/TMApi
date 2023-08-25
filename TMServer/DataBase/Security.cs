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
        private static string Salt = "NySzq6hatK";
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

        public static DBAes? GetAesKeysByCryptId(int cryptId)
        {
            using var db = new TmdbContext();
            return db.AesCrypts.Find(cryptId);
        }

        public static int GetUserId(string login, string password)
        {
            var saltedPassword = GetPasswordWithSalt(password, login);
            using var db = new TmdbContext();
            var user = db.Users.SingleOrDefault(u => u.Login == login && u.Password == saltedPassword);
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

                var dbCrypt = new DBAes()
                {
                    AesKey = aes.Key,
                    IV = aes.IV,
                };
                db.AesCrypts.Add(dbCrypt);
                db.SaveChanges();

                db.Users.Add(new DBUser()
                {
                    Login = login,
                    LastRequest = DateTime.UtcNow,
                    RegisterDate = DateTime.UtcNow,
                    Name = login,
                    CryptId = dbCrypt.CryptId,
                    Password = GetPasswordWithSalt(password, login),
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
                var aes = new DBAes()
                {
                    AesKey = aesKey,
                    IV = aesIV,
                };
                db.AesCrypts.Add(aes);
                db.SaveChanges();
                return aes.CryptId;
            }
            exist.AesKey = aesKey;
            exist.IV = aesIV;
            db.SaveChanges();
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
                db.Tokens.Add(new DBToken()
                {
                    AccessToken = token,
                    UserId = userId,
                    Expiration = expiration
                });
            }
            db.SaveChanges();
        }

        private static string GetPasswordWithSalt(string password, string login)
        {
            return HashGenerator.GenerateHash(password + login + Salt);
        }

        public static bool IsTokenCorrect(string token, int userId)
        {
            using var db = new TmdbContext();
            var dbToken = db.Tokens.SingleOrDefault(t => t.UserId == userId);
            if (dbToken != null)
                return dbToken.AccessToken.Equals(token) && DateTime.UtcNow < dbToken.Expiration;
            return false;
        }
        public static void UpdateAuth(int cryptId, byte[] aesIV)
        {
            using var db = new TmdbContext();

            var aes = db.AesCrypts.Find(cryptId);

            aes.IV = aesIV;

            db.SaveChanges();
        }
        public static AesEncrypter GetAesEncrypter(int cryptId)
        {
            using var db = new TmdbContext();

            var dbAes = db.AesCrypts.SingleOrDefault(a => a.CryptId == cryptId);
            ArgumentNullException.ThrowIfNull(dbAes);

            return new AesEncrypter(dbAes.AesKey, dbAes.IV);
        }
    }
}
