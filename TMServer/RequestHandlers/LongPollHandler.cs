using ApiTypes;
using ApiTypes.Communication.LongPolling;
using ApiTypes.Communication.Packets;
using CSDTP.Packets;
using CSDTP.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Interaction;
using TMServer.DataBase.Tables.LongPolling;

namespace TMServer.RequestHandlers
{
    internal class LongPollHandler
    {
        public static Notification GetUpdates(int userId)
        {
            return new Notification()
            {
                NewMessagesIds = LongPolling.GetMessageUpdate(userId),
                MessagesReadedIds=LongPolling.GetMessagesWithUpdatedStatus(userId),
                FriendRequestIds=LongPolling.GetFriendRequestUpdates(userId),
                NewFriends=LongPolling.GetNewFriends(userId),
                RemovedFriends=LongPolling.GetRemovedFriends(userId),
                NewChats=LongPolling.GetNewChats(userId),
            };
        }

        public static bool IsHaveNotifications(int userId)
        {
            return LongPolling.IsHaveUpdates(userId);
        }
    }
}
