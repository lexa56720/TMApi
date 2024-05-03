using ApiTypes;
using ApiTypes.Communication.Packets;
using CSDTP;
using CSDTP.Cryptography.Providers;
using CSDTP.Protocols;
using CSDTP.Requests;
using TMServer.DataBase.Interaction;
using TMServer.Logger;

namespace TMServer.ServerComponent.Basics
{
    internal abstract class Server : Startable, IDisposable
    {
        protected Responder Responder { get; }
        protected ILogger Logger { get; }

        public virtual int ListenPort => Responder.ListenPort;

        public required Security Security { private get; init; }
        public required Users Users { private get; init; }


        protected bool IsDisposed;

        protected Server(int port, ILogger logger, Protocol protocol = Protocol.Udp)
        {
            Responder = ResponderFactory.Create(port, protocol);
            Logger = logger;
        }
        protected Server(int port, IEncryptProvider encryptProvider, ILogger logger, Protocol protocol = Protocol.Udp)
        {
            if (port == 0)
                Responder = ResponderFactory.Create(encryptProvider, typeof(TMPacket<>), protocol);
            else
                Responder = ResponderFactory.Create(port, encryptProvider, typeof(TMPacket<>), protocol);
            Logger = logger;
        }
        protected Server(IEncryptProvider encryptProvider, ILogger logger, Protocol protocol = Protocol.Udp)
        {
            Responder = ResponderFactory.Create(encryptProvider, typeof(TMPacket<>), protocol);
            Logger = logger;
        }
        public virtual void Dispose()
        {
            if (IsDisposed)
                return;
            Responder.Dispose();
            Security.Dispose();
            Users.Dispose();
            IsDisposed = true;
        }

        public override async Task Start()
        {
            if (IsRunning)
                return;
            await base.Start();

            await Responder.Start();
            Logger.Log($"{GetType().Name} started on port {Responder.ListenPort} {Responder.Protocol}");
        }
        public override async Task Stop()
        {
            if (!IsRunning)
                return; 
            await base.Stop();

            await Responder.Stop();
            Logger.Log($"{GetType().Name} stopped on port {Responder.ListenPort} {Responder.Protocol}");
        }


        protected virtual async Task<bool> IsRequestLegal<T>(ApiData<T> request) where T : ISerializable<T>, new()
        {
            var isLegal = Security.IsTokenCorrect(request.Token, request.UserId);
            if (!isLegal)
                Logger.Log($"illegal request from {request.UserId}");
            else
                await Users.UpdateOnlineStatus(request.UserId);
            return isLegal;
        }
    }
}
