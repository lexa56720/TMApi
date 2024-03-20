using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase;
using TMServer.Logger;

namespace TMServer.Services
{
    internal class TokenCleaner : Service
    {
        public TokenCleaner(TimeSpan period, ILogger logger) : base(period, logger)
        {
        }

        protected override async Task Work(ILogger logger)
        {
            using var db = new TmdbContext();
            var now = DateTime.UtcNow;
            var result = await db.Tokens.Where(t => t.Expiration < now).ExecuteDeleteAsync();
            await db.SaveChangesAsync();
            logger.Log($"TokenCleaner removed {result} old tokens");
        }
    }
}
