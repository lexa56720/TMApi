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
    internal class LongPollCleaner : Service
    {
        private readonly TimeSpan lifeTime;


        public LongPollCleaner(TimeSpan lifeTime,TimeSpan period, ILogger logger) : base(period, logger)
        {
            this.lifeTime = lifeTime;
        }

        protected override async Task Work(ILogger logger)
        {
            using var db = new TmdbContext();
            var now = DateTime.UtcNow;
            var result = await db.LongPollRequests.Where(t => (now-t.CreateDate)<lifeTime).ExecuteDeleteAsync();
            await db.SaveChangesAsync();
            logger.Log($"LongPollCleaner removed {result} old requests");
        }
    }
}
