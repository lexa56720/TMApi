using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase
{
    internal static class Security
    {
        public static bool SaveRsaKeyPair(uint ip, string serverPrivateKey, string clientPublicKey)
        {

        }

        public static KeyPair? GetRsaKeysByIp(uint ip)
        {

        }

        public static AesData? GetAesKeysByCryptId(int cryptId)
        {

        }

        public static int GetUserId(string login, string password)
        {

        }

        public static bool IsLoginFree(string login)
        {

        }

        public static bool CreateUser(string login, string password)
        {

        }

        public static int SaveAuth(int id, byte[] aesKey, byte[] aesIV, string token, DateTime expiration)
        {

        }

        public static void UpdateAuth(int id, byte[] aesIV)
        {

        }
    }
}
