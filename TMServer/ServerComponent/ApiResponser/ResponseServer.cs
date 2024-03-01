using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using CSDTP;
using CSDTP.Cryptography.Providers;
using CSDTP.Packets;
using CSDTP.Requests;
using TMServer.DataBase;
using TMServer.DataBase.Interaction;
using TMServer.Logger;
using TMServer.ServerComponent.Basics;

namespace TMServer.ServerComponent.ApiResponser
{
    internal class ResponseServer : Server
    {
        public ResponseServer(int port, IEncryptProvider encryptProvider, ILogger logger) : base(port, encryptProvider, logger)
        {
        }

        public void RegisterRequestHandler<TRequest, TResponse>(Func<ApiData<TRequest>, TResponse?> func)
                    where TRequest : ISerializable<TRequest>, new()
                    where TResponse : ISerializable<TResponse>, new()
        {
            Responder.RegisterRequestHandler(Invoke(func));
        }
        public void RegisterRequestHandler<TRequest, TResponse>(Func<ApiData<TRequest>, IPacketInfo, TResponse?> func)
                    where TRequest : ISerializable<TRequest>, new()
                    where TResponse : ISerializable<TResponse>, new()
        {
            Responder.RegisterRequestHandler(Invoke(func));
        }


        public void RegisterDataHandler<TRequest>(Action<ApiData<TRequest>> func)
                    where TRequest : ISerializable<TRequest>, new()
        {
            Responder.RegisterDataHandler(Invoke(func));
        }
        public void RegisterDataHandler<TRequest>(Action<ApiData<TRequest>, IPacketInfo> func)
                  where TRequest : ISerializable<TRequest>, new()
        {
            Responder.RegisterDataHandler(Invoke(func));
        }


        private Action<ApiData<TRequest>> Invoke<TRequest>(Action<ApiData<TRequest>> func)
                                            where TRequest : ISerializable<TRequest>, new()
        {
            return new Action<ApiData<TRequest>>((request) =>
            {
                if (IsRequestLegal(request))
                {
                    Logger.Log(request);
                    func(request);
                }
            });
        }
        private Action<ApiData<TRequest>, IPacketInfo> Invoke<TRequest>(Action<ApiData<TRequest>,IPacketInfo> func)
                                    where TRequest : ISerializable<TRequest>, new()
        {
            return new Action<ApiData<TRequest>, IPacketInfo>((request,info) =>
            {
                if (IsRequestLegal(request))
                {
                    Logger.Log(request);
                    func(request,info);
                }
            });
        }


        private Func<ApiData<TRequest>, TResponse?> Invoke<TRequest, TResponse>(Func<ApiData<TRequest>, TResponse?> func)
                                                    where TRequest : ISerializable<TRequest>, new()
                                                    where TResponse : ISerializable<TResponse>, new()
        {
            return new Func<ApiData<TRequest>, TResponse?>((request) =>
            {
                if (IsRequestLegal(request))
                {
                    Logger.Log(request);
                    return func(request);
                }
                return default;
            });
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
