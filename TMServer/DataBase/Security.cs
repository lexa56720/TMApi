using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase
{
    internal static class Security
    {
        public static void SaveRsaKeyPair(uint ip, string serverPrivateKey, string clientPublicKey)
        {

        }

        public static KeyPair? GetRsaKeysByIp(uint ip)
        {

        }

        public static int GetUserId(string login, string password)
        {

        }

        public static void SaveAuth(int id, byte[] aesKey, byte[] aesIV, string token, DateTime expiration)
        {

        }

        public static void UpdateAuth(int id, byte[] aesIV)
        {

        }
    }
}
