using ApiTypes.Shared;
using Microsoft.EntityFrameworkCore;
using PerformanceUtils.Collections;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase.Interaction
{
    public class Authentications
    {
        private readonly string Salt;

        public Authentications(string salt)
        {
            Salt = salt;
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
        public async Task<DBUser> CreateUser(string username, string login, string password)
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
            return user;
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
