using ApiTypes.Shared;
using PerformanceUtils.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.MemoryEntities;

namespace TMServer.DataBase.Interaction
{
    public class Tokens : IDisposable
    {
        private readonly TimeSpan TokenLifeTime;
        private readonly LifeTimeDictionary<int, List<RamToken>> UserTokens;

        private bool IsDisposed = false;

        public Tokens(TimeSpan tokenLifeTime)
        {
            TokenLifeTime = tokenLifeTime;
            UserTokens = new LifeTimeDictionary<int, List<RamToken>>(l=>l?.Clear());
        }
        public void Dispose()
        {
            if (IsDisposed)
                return;
            UserTokens.Clear();
            IsDisposed = true;
        }

        public IEnumerable<RamToken> GetTokens(int userId)
        {
            if (UserTokens.TryGetValue(userId, out var tokens))
                return tokens;
            return [];
        }
        public RamToken AddToken(int userId)
        {
            using var db = new TmdbContext();
            var dbToken = new RamToken()
            {
                AccessToken = HashGenerator.GetRandomString(),
                UserId = userId,
                Expiration = DateTime.UtcNow + TokenLifeTime,
            };
            if (UserTokens.TryGetValue(userId, out var tokens))
                tokens.Add(dbToken);
            else
                UserTokens.TryAdd(userId, [dbToken], TokenLifeTime);
            return dbToken;
        }
    }
}
