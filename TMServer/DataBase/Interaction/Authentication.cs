using ApiTypes.Shared;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase.Interaction
{
    public static class Authentication
    {
        private static string Salt = GlobalSettings.PasswordSalt;

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
        public static void CreateUser(string username, string login, string password, byte[] aesKey)
        {
            using var db = new TmdbContext();

            var user = new DBUser()
            {
                Login = login,
                LastRequest = DateTime.UtcNow,
                RegisterDate = DateTime.UtcNow,
                Name = username,
                Password = GetPasswordWithSalt(password, login),
            };
            db.Users.Add(user);
            db.SaveChanges();

            db.AesCrypts.Add(new DBAes()
            {
                AesKey = aesKey,
                UserId = user.Id,
            });
            db.SaveChanges();
        }

        public static int SaveAuth(int userId, byte[] aesKey, string token, DateTime expiration)
        {
            AddToken(userId, token, expiration);
            return AddAes(userId, aesKey);
        }
        private static int AddAes(int userId, byte[] aesKey)
        {
            using var db = new TmdbContext();

            var aes = new DBAes()
            {
                AesKey = aesKey,
                UserId = userId,
            };
            db.AesCrypts.Add(aes);
            db.SaveChanges();
            return aes.Id;
        }
        private static void AddToken(int userId, string token, DateTime expiration)
        {
            using var db = new TmdbContext();

            db.Tokens.Add(new DBToken()
            {
                AccessToken = token,
                UserId = userId,
                Expiration = expiration
            });
            db.SaveChanges();
        }

        private static string GetPasswordWithSalt(string password, string login)
        {
            return HashGenerator.GenerateHash(password + login + Salt);
        }
    }
}
