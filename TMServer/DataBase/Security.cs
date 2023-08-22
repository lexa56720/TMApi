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
                PublicClientKey = clientPublicKey
            });
            db.SaveChanges();
        }

        public static RsaCrypt? GetRsaKeysByIp(uint ip)
        {
            using var db = new TmdbContext();
            return db.RsaCrypts.First(rsa => rsa.Ip == ip);
        }

        public static AesData? GetAesKeysByCryptId(int cryptId)
        {
            throw new NotImplementedException();
        }

        public static int GetUserId(string login, string password)
        {
            throw new NotImplementedException();
        }

        public static bool IsLoginFree(string login)
        {
            throw new NotImplementedException();
        }

        public static bool CreateUser(string login, string password)
        {
            throw new NotImplementedException();
        }

        public static int SaveAuth(int id, byte[] aesKey, byte[] aesIV, string token, DateTime expiration)
        {
            throw new NotImplementedException();
        }

        public static void UpdateAuth(int cryptId, byte[] aesIV)
        {
            throw new NotImplementedException();
        }
    }
}
