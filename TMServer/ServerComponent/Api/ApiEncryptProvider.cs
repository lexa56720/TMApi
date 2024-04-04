using ApiTypes.Communication.Packets;
using CSDTP.Cryptography.Algorithms;
using CSDTP.Cryptography.Providers;
using CSDTP.Packets;
using System.Net.Sockets;
using TMServer.DataBase.Interaction;

namespace TMServer.ServerComponent.Api
{
    internal class ApiEncryptProvider : IEncryptProvider
    {
        private readonly Crypt Crypt;

        public ApiEncryptProvider(Crypt crypt)
        {
            Crypt = crypt;
        }
        public void Dispose()
        {
            return;
        }

        public void DisposeEncrypter(IEncrypter encrypter)
        {
            encrypter.Dispose();
        }
        public IEncrypter? GetDecrypter(ReadOnlySpan<byte> bytes)
        {
            var cryptId = BitConverter.ToInt32(bytes.Slice(bytes.Length - 4, 4));
            var key = Crypt.GetAesKey(cryptId);
            if (key == null)
                return null;
            return new AesEncrypter(key);
        }

        public IEncrypter? GetEncrypter(IPacketInfo responsePacket, IPacketInfo? requestPacket = null)
        {
            if (requestPacket is not ITMPacket reqPacket || responsePacket is not ITMPacket resPacket)
                return null;
            resPacket.Id = reqPacket.Id;
            var key = Crypt.GetAesKey(resPacket.Id);
            if (key == null)
                return null;
            return new AesEncrypter(key);
        }
    }
}
