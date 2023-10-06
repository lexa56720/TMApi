﻿using ApiTypes;
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
using TMServer.DataBase.Interaction;
using TMServer.Logger;
using TMServer.RequestHandlers;
using TMServer.ServerComponent.Basics;

namespace TMServer.ServerComponent.LongPolling
{
    internal class LongPollingServer : Server
    {
        public LongPollingServer(int port, IEncryptProvider twoWayProvider, ILogger logger) : base(port, twoWayProvider, logger)
        {
            Responder.ResponseIfNull = false;
            Responder.RegisterPostHandler<ApiData<LongPollingRequest>, Notification>(LongPollArrived);
        }


        private Notification? LongPollArrived(ApiData<LongPollingRequest> request, IPacketInfo info)
        {
            if (!IsRequestLegal(request))
                return null;

            var packet = info as IPacket;

            SaveToDB(request.UserId, packet);
            return null;
        }
        private void SaveToDB(int userId, IPacket packet)
        {
            using var ms = new MemoryStream();
            packet.Serialize(new BinaryWriter(ms));
            LongPollHandler.SaveToDB(userId, ms.ToArray());
        }
        private IPacket LoadFromDB(int userId)
        {
            var data = LongPollHandler.LoadFromDB(userId);
            using var ms = new MemoryStream(data.RequestPacket);
            throw new NotImplementedException();
        }
        public void Respond(int userId)
        {
            var packet = LoadFromDB(userId);
            var requestContainer = packet.DataObj as IRequestContainer;

            Responder.ResponseManually(requestContainer, packet, new Notification());
        }

        private bool IsRequestLegal<T>(ApiData<T> request) where T : ISerializable<T>
        {
            var isLegal = Security.IsTokenCorrect(request.Token, request.UserId);
            if (!isLegal)
                Logger.Log($"illegal request from {request.UserId}");
            else
                Users.UpdateLastRequest(request.UserId);
            return isLegal;
        }
    }
}
