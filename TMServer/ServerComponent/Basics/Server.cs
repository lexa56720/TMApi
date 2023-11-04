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
        protected Responder Responder { get; set; }
        protected ILogger Logger { get; }

        protected Server(int port, IEncryptProvider encryptProvider, IEncryptProvider decryptProvider, ILogger logger)
        {
            Responder = new Responder(TimeSpan.FromSeconds(20), port, encryptProvider, decryptProvider);
            Responder.SetPacketType(typeof(TMPacket<>));
            Logger = logger;
        }

        protected Server(int port, IEncryptProvider twoWayProvider,ILogger logger)
        {
            Responder = new Responder(TimeSpan.FromSeconds(20), port, twoWayProvider, twoWayProvider);
            Responder.SetPacketType(typeof(TMPacket<>));
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
            Logger.Log($"{GetType().Name} started");
        }
        public override void Stop()
        {
            Responder.Stop(); 
            Logger.Log($"{GetType().Name} stopped");
        }
        protected virtual bool IsRequestLegal<T>(ApiData<T> request) where T : ISerializable<T>
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
