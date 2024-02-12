using ApiTypes.Communication.Packets;
using CSDTP.Cryptography.Algorithms;
using CSDTP.Cryptography.Providers;
using CSDTP.Packets;
using TMServer.DataBase.Interaction;
using TMServer.DataBase.Tables;

namespace TMServer.ServerComponent.Auth
{
    internal class AuthEncryptProvider : IEncryptProvider
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void DisposeEncrypter(IEncrypter encrypter)
        {
            encrypter.Dispose();
        }

        public IEncrypter? GetDecrypter(ReadOnlySpan<byte> bytes)
        {
            DBRsa? keys = null;
            if (keys == null)
                return null;
            var rsaDecrypter = new RsaEncrypter(keys.PrivateServerKey);
            return rsaDecrypter;
        }
        public IEncrypter? GetEncrypter(IPacketInfo packet)
        {
            if (IsInitPacket(packet))
                return new FakeEncrypter();

            var keys = GetKeys((IPacketInfo)packet.InfoObj);
            if (keys == null)
                return null;
            var rsaDecrypter = new RsaEncrypter(keys.PublicClientKey);
            return rsaDecrypter;
        }

        private DBRsa? GetKeys(IPacketInfo packet)
        {
            return Crypt.GetRsaKeysById(((ITMPacket)packet).Id.InstanceValue);
        }
        private bool IsInitPacket(IPacketInfo packet)
        {
            if (((ITMPacket)(IPacketInfo)packet.InfoObj).Id.InstanceValue <= 0)
                return true;
            return false;
        }
    }
}
