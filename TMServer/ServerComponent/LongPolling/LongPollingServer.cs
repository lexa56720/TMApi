using ApiTypes;
using ApiTypes.Communication.LongPolling;
using CSDTP;
using CSDTP.Cryptography.Providers;
using CSDTP.Packets;
using CSDTP.Requests;
using CSDTP.Requests.RequestHeaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase;
using TMServer.DataBase.Interaction;
using TMServer.Logger;
using TMServer.RequestHandlers;
using TMServer.ServerComponent.Basics;

namespace TMServer.ServerComponent.LongPolling
{
    internal class LongPollingServer : Server
    {
        public LongPollingServer(int port, IEncryptProvider twoWayProvider, ILogger logger) 
                              : base(port, twoWayProvider, logger)
        {
           // Responder.ResponseIfNull = false;
            Responder.RegisterRequestHandler<ApiData<LongPollingRequest>, Notification>(LongPollArrived);
            DBChangeHandler.UpdateForUser += DBUpdateForUser;
        }

        public override void Dispose()
        {
            base.Dispose();
            DBChangeHandler.UpdateForUser -= DBUpdateForUser;
        }
        private void DBUpdateForUser(object? sender, int userId)
        {
            RespondOnSaved(userId);
        }

        private Notification? LongPollArrived(ApiData<LongPollingRequest> request, IPacketInfo info)
        {
            Logger.Log($"poll request from {request.UserId} arrived");

            if (!IsRequestLegal(request))
                return null;

            if (LongPollHandler.IsHaveNotifications(request.UserId))
                return LongPollHandler.GetUpdates(request.UserId);

            LongPollHandler.SaveToDB(request.UserId, (IPacket)info);
            return null;
        }

        public void RespondOnSaved(int userId)
        {      
            var packet = LongPollHandler.LoadFromDB(userId);         
            if (packet == null)
                return;
            var requestContainer = packet.DataObj as IRequestContainer;

            Logger.Log($"poll request from {userId} responsed");

            var notification = LongPollHandler.GetUpdates(userId);
           // Responder.ResponseManually(requestContainer, packet, notification);
        }
    }
}
