using ApiTypes.Shared;
using Microsoft.EntityFrameworkCore;
using PerformanceUtils.Collections;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase.Interaction
{
    public class Authentication
    {
        private readonly string Salt;

        private readonly TimeSpan TokenLifeTime;

        public Authentication(string salt, TimeSpan tokenLifeTime)
        {
            Salt = salt;
            TokenLifeTime = tokenLifeTime;
        }

        public async Task<int> GetUserId(string login, string password)
        {
            var saltedPassword = GetPasswordWithSalt(password);
            using var db = new TmdbContext();
            var user = await db.Users.SingleOrDefaultAsync(u => u.Login == login);
            if (user != null && user.Password == saltedPassword)
                return user.Id;
            return -1;
        }
        public async Task<bool> IsLoginAvailable(string login)
        {
            using var db = new TmdbContext();
            return !await db.Users.AnyAsync(u => u.Login == login);
        }
        public async Task CreateUser(string username, string login, string password, byte[] aesKey)
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
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();

            await AddAes(user.Id, aesKey);
        }

        public async Task<(DBToken token,DBAes aes)> SaveAuth(int userId, byte[] aesKey)
        {
            var token = HashGenerator.GetRandomString();
            var expiration = DateTime.UtcNow.Add(TokenLifeTime);
            var dbToken = await AddToken(userId, token, expiration);
            var dbAes = await AddAes(userId, aesKey);
            return (dbToken,dbAes);
        }
        private async Task<DBAes> AddAes(int userId, byte[] aesKey)
        {
            using var db = new TmdbContext();

            var aes = new DBAes()
            {
                AesKey = aesKey,
                Expiration = DateTime.MaxValue,
                UserId = userId,
            };
            await db.AesCrypts.AddAsync(aes);
            await db.SaveChangesAsync();
            return aes;
        }
        private async Task<DBToken> AddToken(int userId, string token, DateTime expiration)
        {
            using var db = new TmdbContext();
            var dbToken = new DBToken()
            {
                AccessToken = token,
                UserId = userId,
                Expiration = expiration
            };
            await db.Tokens.AddAsync(dbToken);
            await db.SaveChangesAsync();
            return dbToken;
        }

        public async Task<bool> IsPasswordMatch(int userId, string password)
        {
            using var db = new TmdbContext();

            var user = await db.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return false;
            return user.Password.Equals(GetPasswordWithSalt(password));
        }
        public async Task<bool> ChangePassword(int userId, string newPassword)
        {
            using var db = new TmdbContext();

            var user = await db.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return false;
            user.Password = GetPasswordWithSalt(newPassword);
            return await db.SaveChangesAsync() > 0;
        }
        private string GetPasswordWithSalt(string password)
        {
            return HashGenerator.GenerateHash(password + Salt);
        }
    }
}
