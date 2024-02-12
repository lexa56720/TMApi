using ApiTypes.Communication.Packets;
using CSDTP.Cryptography.Algorithms;
using CSDTP.Cryptography.Providers;
using CSDTP.Packets;
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
            var cryptId = 0;
            //var cryptId = ((ITMPacket)packet.InfoObj).Id.InstanceValue;
            var key = Crypt.GetAesKey(cryptId);
            if (key == null)
                return null;
            return new AesEncrypter(key);
        }

        public IEncrypter? GetEncrypter(IPacketInfo packet)
        {
            var cryptId = ((ITMPacket)packet.InfoObj).Id.InstanceValue;
            var key = Crypt.GetAesKey(cryptId); 
            if (key == null)
                return null;
            return new AesEncrypter(key);
        }
    }
}
