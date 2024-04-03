using ApiTypes.Shared;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase.Interaction
{
    public class Authentication
    {
        private readonly string Salt;

        public Authentication(string salt)
        {
            Salt = salt;
        }

        public int GetUserId(string login, string password)
        {
            var saltedPassword = GetPasswordWithSalt(password);
            using var db = new TmdbContext();
            var user = db.Users.SingleOrDefault(u => u.Login == login && u.Password == saltedPassword);
            return user == null ? -1 : user.Id;
        }
        public bool IsLoginAvailable(string login)
        {
            using var db = new TmdbContext();
            return !db.Users.Any(u => u.Login == login);
        }
        public void CreateUser(string username, string login, string password, byte[] aesKey)
        {
            using var db = new TmdbContext();

            var user = new DBUser()
            {
                Login = login,
                LastRequest = DateTime.UtcNow,
                RegisterDate = DateTime.UtcNow,
                Name = username,
                Password = GetPasswordWithSalt(password),
            };
            db.Users.Add(user);
            db.SaveChanges();

            AddAes(user.Id, aesKey);
        }

        public int SaveAuth(int userId, byte[] aesKey, string token, DateTime expiration)
        {
            AddToken(userId, token, expiration);
            return AddAes(userId, aesKey);
        }
        private int AddAes(int userId, byte[] aesKey)
        {
            using var db = new TmdbContext();

            var aes = new DBAes()
            {
                AesKey = aesKey,
                Expiration = DateTime.MaxValue,
                UserId = userId,
            };
            db.AesCrypts.Add(aes);
            db.SaveChanges();
            return aes.Id;
        }
        private void AddToken(int userId, string token, DateTime expiration)
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

        public bool IsPasswordMatch(int userId, string password)
        {
            using var db = new TmdbContext();

            var user = db.Users.SingleOrDefault(u => u.Id == userId);
            if (user == null)
                return false;
            return user.Password.Equals( GetPasswordWithSalt(password));
        }
        public bool ChangePassword(int userId, string newPassword)
        {
            using var db = new TmdbContext();

            var user = db.Users.SingleOrDefault(u => u.Id == userId);
            if (user == null)
                return false;
            user.Password=GetPasswordWithSalt(newPassword);
            return db.SaveChanges() > 0;
        }
        private string GetPasswordWithSalt(string password)
        {
            return HashGenerator.GenerateHash(password + Salt);
        }
    }
}
