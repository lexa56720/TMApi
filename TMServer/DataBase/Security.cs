using ApiTypes.Shared;
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
                CreateDate =DateTime.UtcNow,
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
            throw new NotImplementedException();
            //using var db = new TmdbContext();
            //if (!db.Users.Any(u => u.Login == login))
            //{
            //    db.Add(new User()
            //    {
            //        Login = login,
            //        Password = password,
            //    });
            //    db.SaveChanges();

            //    return true;
            //}
            //return false;

        }

        public static int SaveAuth(int id, byte[] aesKey, byte[] aesIV, string token, DateTime expiration)
        {
            using var db = new TmdbContext();
            //token check needed
            db.Update(new AesCrypt()
            {
                AesKey = HashGenerator.BytesToString(aesKey),
                IV = HashGenerator.BytesToString(aesIV),
                CryptId = id,

            });
            throw new NotImplementedException();
        }

        public static void UpdateAuth(int cryptId, byte[] aesIV)
        {
            throw new NotImplementedException();
        }
    }
}
