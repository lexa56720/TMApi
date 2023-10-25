using ApiTypes.Communication.LongPolling;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Tables.LongPolling;

namespace TMServer.DataBase.Interaction
{
    internal static class LongPolling
    {
        public static int[] GetChatUpdate(int userId)
        {
            using var db = new TmdbContext();
            var result = db.ChatUpdates
                           .Where(c => c.UserId == userId)
                           .Select(m => m.MessageId)
                           .ToArray();
            db.RemoveRange(result);
            db.SaveChanges();
            return result;
        }
        public static void SaveRequest(int userId, byte[] data, string dataType)
        {
            using var db = new TmdbContext();
            db.LongPollRequests.Where(r => r.UserId == userId).ExecuteDelete();
            db.LongPollRequests.Add(new DBLongPollRequest()
            {
                RequestPacket = data,
                UserId = userId,
                DataType = dataType
            });
            db.SaveChanges();
        }
        public static DBLongPollRequest LoadRequest(int userId)
        {
            using var db = new TmdbContext();
            var result = db.LongPollRequests
                           .SingleOrDefault(r => r.UserId == userId);
            db.LongPollRequests.Remove(result);
            return result;
        }

    }
}
