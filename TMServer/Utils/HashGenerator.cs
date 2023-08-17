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
        public static string GetRandomString()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));
        }
    }
}
