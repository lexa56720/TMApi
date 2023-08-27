using CSDTP;
using CSDTP.Cryptography.Providers;
using CSDTP.Packets;
using TMServer.ServerComponent.Basics;

namespace TMServer.ServerComponent.Auth
{
    internal class AuthorizationServer : Server
    {
        public AuthorizationServer(int port, IEncryptProvider encryptProvider) : base(port, encryptProvider)
        {
        }

        public void Register<T, U>(Func<T, U> func) where T : ISerializable<T> where U : ISerializable<U>
        {
            Responder.RegisterPostHandler(func);
        }
        public void Register<T, U>(Func<T, IPacketInfo, U> func) where T : ISerializable<T> where U : ISerializable<U>
        {
            Responder.RegisterPostHandler(func);
        }
    }
}
