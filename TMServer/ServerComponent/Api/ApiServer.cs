using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using CSDTP;
using CSDTP.Cryptography.Providers;
using CSDTP.Packets;
using CSDTP.Protocols;
using CSDTP.Requests;
using TMServer.DataBase;
using TMServer.DataBase.Interaction;
using TMServer.Logger;
using TMServer.ServerComponent.Basics;

namespace TMServer.ServerComponent.Api
{
    internal class ApiServer(int port,IEncryptProvider encryptProvider, ILogger logger,Protocol protocol=Protocol.Udp)
                   : Server(port,encryptProvider, logger,protocol)
    {
        public void RegisterRequestHandler<TRequest, TResponse>(Func<ApiData<TRequest>, TResponse?> func)
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
    }
}
