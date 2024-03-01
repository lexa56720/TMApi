using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.LongPolling;
using CSDTP;
using CSDTP.Cryptography.Providers;
using CSDTP.Packets;
using CSDTP.Requests;
using CSDTP.Requests.RequestHeaders;
using PerformanceUtils.Collections;
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
    internal class LongPollServer : Server
    {

        private readonly LifeTimeDictionary<int, IPacket<IRequestContainer>> Requests = new();

        private readonly TimeSpan LongPollLifetime;

        public LongPollServer(TimeSpan longPollLifetime, int port, IEncryptProvider encryptProvider, ILogger logger) : base(port, encryptProvider, logger)
        {

            TmdbContext.ChangeHandler.UpdateForUser += DBUpdateForUser;
            LongPollLifetime = longPollLifetime;
        }

        public override void Dispose()
        {
            base.Dispose();
            TmdbContext.ChangeHandler.UpdateForUser -= DBUpdateForUser;
            Requests.Clear();
        }
        private void DBUpdateForUser(object? sender, int userId)
        {
            RespondOnSaved(userId);
        }

        public Notification? LongPollArrived(ApiData<LongPollingRequest> request, IPacketInfo info)
        {
            if (!IsRequestLegal(request))
                return null;


            if (LongPollHandler.IsHaveNotifications(request.UserId))
                return LongPollHandler.GetUpdates(request.UserId);

            Requests.TryRemove(request.UserId, out _);
            Requests.TryAdd(request.UserId, (IPacket<IRequestContainer>)info, LongPollLifetime);
            return null;
        }
        public void RespondOnSaved(int userId)
        {
            if (!Requests.TryGetValue(userId, out var packet))
                return;

            var notification = LongPollHandler.GetUpdates(userId);
            Responder.ResponseManually(packet, notification);
        }


        public void RegisterRequestHandler<TRequest, TResponse>(Func<ApiData<TRequest>, IPacketInfo, TResponse?> func)
            where TRequest : ISerializable<TRequest>, new()
            where TResponse : ISerializable<TResponse>, new()
        {
            Responder.RegisterRequestHandler(Invoke(func));
        }

        private Func<ApiData<TRequest>, IPacketInfo, TResponse?> Invoke<TRequest, TResponse>(Func<ApiData<TRequest>, IPacketInfo, TResponse?> func)
                                                         where TRequest : ISerializable<TRequest>, new()
                                                         where TResponse : ISerializable<TResponse>, new()
        {
            return new Func<ApiData<TRequest>, IPacketInfo, TResponse?>((request, info) =>
            {
                if (IsRequestLegal(request))
                {
                    Logger.Log(request);
                    return func(request, info);
                }
                return default;
            });
        }
    }
}
