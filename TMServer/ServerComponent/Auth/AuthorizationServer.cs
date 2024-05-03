using CSDTP;
using CSDTP.Cryptography.Providers;
using CSDTP.Packets;
using TMServer.Logger;
using TMServer.ServerComponent.Basics;

namespace TMServer.ServerComponent.Auth
{
    internal class AuthorizationServer(int port, IEncryptProvider encryptProvider, ILogger logger) : Server(port, encryptProvider, logger)
    {
        public void Register<TRequest, TResponse>(Func<TRequest, Task<TResponse?>> func)
                    where TRequest : ISerializable<TRequest>, new()
                    where TResponse : ISerializable<TResponse>, new()
        {
            Responder.RegisterRequestHandler(Invoke(func));
        }
        private Func<TRequest, Task<TResponse?>> Invoke<TRequest, TResponse>(Func<TRequest, Task<TResponse?>> func)
                                          where TRequest : ISerializable<TRequest>, new()
                                          where TResponse : ISerializable<TResponse>, new()
        {
            return new Func<TRequest, Task<TResponse?>>(async o =>
            {
                Logger.Log($"authorization request");
                return await func(o);
            });
        }
    }
}
