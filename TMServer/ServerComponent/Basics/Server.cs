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

        protected Server(int port, IEncryptProvider encryptProvider, ILogger logger, Protocol protocol = Protocol.Udp)
        {
            Responder = ResponderFactory.Create(port, encryptProvider, typeof(TMPacket<>), protocol);
            Logger = logger;
        }
        public virtual void Dispose()
        {
            Responder.Dispose();
        }

        public override void Start()
        {
            base.Start();

            Responder.Start();
            Logger.Log($"{GetType().Name} started on port {Responder.ListenPort} {Responder.Protocol}");
        }
        public override void Stop()
        {
            Responder.Stop();
            Logger.Log($"{GetType().Name} stopped on port {Responder.ListenPort} {Responder.Protocol}");
        }


        protected virtual bool IsRequestLegal<T>(ApiData<T> request) where T : ISerializable<T>, new()
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
