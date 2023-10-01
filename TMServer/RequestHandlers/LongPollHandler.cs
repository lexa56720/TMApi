using ApiTypes;
using ApiTypes.Communication.LongPolling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Tables;

namespace TMServer.RequestHandlers
{
    internal class LongPollHandler
    {
        public static Notification GetUpdates(ApiData<LongPollingRequest> request)
        {
            var n = new Notification();
            return n;
        }

        public static void SaveToDB(int userId,byte[] bytes)
        {
            throw new NotImplementedException();
        }
        public static DBLongPoll LoadFromDB(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
