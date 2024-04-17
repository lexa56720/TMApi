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
        private readonly Crypt Crypt;

        public AuthEncryptProvider(Crypt crypt) 
        {
            Crypt = crypt;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void DisposeEncrypter(IEncrypter encrypter)
        {
            encrypter.Dispose();
        }

        public async Task<IEncrypter?> GetDecrypter(Memory<byte> bytes)
        {
            var cryptId = BitConverter.ToInt32(bytes.Slice(bytes.Length - sizeof(int), sizeof(int)).Span);
            if (cryptId == 0)
                return null;
            var keys =await Crypt.GetRsaKeysById(cryptId);
            if (keys == null)
                return null;
            var rsaDecrypter = new RsaEncrypter(keys.PrivateServerKey);
            return rsaDecrypter;
        }
        public async Task<IEncrypter?> GetEncrypter(IPacketInfo responsePacket, IPacketInfo? requestPacket)
        {
            if (requestPacket == null || IsInitPacket(requestPacket))
                return null;

            var keys =await GetKeys(requestPacket);
            if (keys == null)
                return null;
            var rsaDecrypter = new RsaEncrypter(keys.PublicClientKey);
            return rsaDecrypter;
        }

        private async Task<DBRsa?> GetKeys(IPacketInfo packet)
        {
            return await Crypt.GetRsaKeysById(((ITMPacket)packet).Id);
        }
        private bool IsInitPacket(IPacketInfo packet)
        {
            if (packet is ITMPacket castedPacket && castedPacket.Id <= 0)
                return true;
            return false;
        }
    }
}
