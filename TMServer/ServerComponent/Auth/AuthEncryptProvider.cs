using ApiTypes.Communication.Packets;
using CSDTP.Cryptography.Algorithms;
using CSDTP.Cryptography.Providers;
using CSDTP.Packets;
using System.Net.Sockets;
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
            var cryptId = BitConverter.ToInt32(bytes.Slice(bytes.Length - sizeof(int), sizeof(int)));
            if (cryptId == 0)
                return null;
            var keys = Crypt.GetRsaKeysById(cryptId);
            if (keys == null)
                return null;
            var rsaDecrypter = new RsaEncrypter(keys.PrivateServerKey);
            return rsaDecrypter;
        }
        public IEncrypter? GetEncrypter(IPacketInfo responsePacket, IPacketInfo? requestPacket)
        {
            if (requestPacket == null || IsInitPacket(requestPacket))
                return null;

            var keys = GetKeys(requestPacket);
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
            if (packet is ITMPacket castedPacket && castedPacket.Id.InstanceValue <= 0)
                return true;
            return false;
        }
    }
}
