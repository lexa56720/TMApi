using System.Security.Cryptography;
using System.Text;

namespace ApiTypes.Shared
{
    public static class HashGenerator
    {
        private static readonly SHA512 Hasher = SHA512.Create();
        public static string GetRandomString()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));
        }
        public static string GetPasswordHash(string password, string login)
        {
            return GenerateHash(password + login);
        }
        public static string GenerateHash(byte[] bytes)
        {
            return Convert.ToBase64String(Hasher.ComputeHash(bytes));
        }

        public static string GenerateHash(string str)
        {
            return Convert.ToBase64String(Hasher.ComputeHash(Encoding.UTF8.GetBytes(str)));
        }


        public static string BytesToString(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static byte[] Base64ToBytes(string str)
        {
            return Convert.FromBase64String(str);
        }
    }
}
