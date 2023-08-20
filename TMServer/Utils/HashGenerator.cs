using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.Utils
{
    internal static class HashGenerator
    {
        static readonly SHA512 Hasher = SHA512.Create();
        public static string GetRandomString()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));
        }

        public static string GenerateHash(byte[] bytes)
        {
            return Convert.ToHexString(Hasher.ComputeHash(bytes));
        }

        public static string GenerateHash(string str)
        {
            return Convert.ToHexString(Hasher.ComputeHash(Encoding.UTF8.GetBytes(str)));
        }
    }
}
