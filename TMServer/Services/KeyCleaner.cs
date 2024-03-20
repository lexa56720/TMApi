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
    internal class KeyCleaner : Service
    {
        public KeyCleaner(TimeSpan period, ILogger logger) : base(period, logger)
        {
        }

        protected override async Task Work(ILogger logger)
        {
            using var db = new TmdbContext();
            var now = DateTime.UtcNow;
            var deletedAes = await db.AesCrypts.Where(a=>a.Expiration<now).ExecuteDeleteAsync();
            var deletedRsa = await db.RsaCrypts.Where(r =>r.Expiration<now).ExecuteDeleteAsync();

            await db.SaveChangesAsync();
            logger.Log($"{GetType().Name} removed {deletedAes} old aes keys");
            logger.Log($"{GetType().Name} removed {deletedRsa} old rsa keys");
        }
    }
}
