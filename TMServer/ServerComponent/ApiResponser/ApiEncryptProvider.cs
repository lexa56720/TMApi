using ApiTypes.Communication.Packets;
using CSDTP.Cryptography.Algorithms;
using CSDTP.Cryptography.Providers;
using CSDTP.Packets;
using System.Net.Sockets;
using TMServer.DataBase.Interaction;

namespace TMServer.ServerComponent.ApiResponser
{
    internal class ApiEncryptProvider : IEncryptProvider
    {
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
            var cryptId = BitConverter.ToInt32(bytes.Slice(bytes.Length-4,4));
            var key = Crypt.GetAesKey(cryptId);
            if (key == null)
                return null;
            return new AesEncrypter(key);
        }

        public IEncrypter? GetEncrypter(IPacketInfo responsePacket, IPacketInfo? requestPacket = null)
        {
            if (requestPacket is not ITMPacket)
                return null;
            var cryptId = ((ITMPacket)requestPacket).Id.InstanceValue;
            var key = Crypt.GetAesKey(cryptId);
            if (key == null)
                return null;
            return new AesEncrypter(key);
        }
    }
}
