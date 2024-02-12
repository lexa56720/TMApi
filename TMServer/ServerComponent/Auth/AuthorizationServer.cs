using CSDTP;
using CSDTP.Cryptography.Providers;
using CSDTP.Packets;
using TMServer.Logger;
using TMServer.ServerComponent.Basics;

namespace TMServer.ServerComponent.Auth
{
    internal class AuthorizationServer : Server
    {
        public AuthorizationServer(int port, IEncryptProvider encryptProvider, ILogger logger) : base(port, encryptProvider,logger)
        {
        }
        public void Register<T, U>(Func<T, U> func) where T : ISerializable<T>, new() where U : ISerializable<U>,new()
        {
            Responder.RegisterRequestHandler(Invoke(func));
        }
        public void Register<T, U>(Func<T, IPacketInfo, U> func) where T : ISerializable<T>, new() where U : ISerializable<U>, new()
        {
            Responder.RegisterRequestHandler(Invoke(func));
        }
        private Func<T, U> Invoke<T, U>(Func<T, U> func) where T : ISerializable<T>, new() where U : ISerializable<U>, new()
        {
            return new Func<T, U>(o=>
            {
                Logger.Log($"auth stuff");
                return func(o);
            });
        }
        private Func<T, IPacketInfo, U> Invoke<T, U>(Func<T, IPacketInfo, U> func) where T : ISerializable<T>, new() where U : ISerializable<U>, new()
        {
            return new Func<T, IPacketInfo, U>((o, e)=>
            {
                Logger.Log($"{e.Source} auth stuff");
                return func(o, e);
            });
        }
    }
}
