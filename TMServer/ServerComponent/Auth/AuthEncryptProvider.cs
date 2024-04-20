using ApiTypes.Communication.Packets;
using CSDTP.Cryptography.Algorithms;
using CSDTP.Cryptography.Providers;
using CSDTP.Packets;
using System.Net.Sockets;
using TMServer.DataBase.Interaction;
using TMServer.DataBase.MemoryEntities;

namespace TMServer.ServerComponent.Auth
{
    internal class AuthEncryptProvider : IEncryptProvider
    {
        private readonly Crypts Crypt;

        public AuthEncryptProvider(Crypts crypt)
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

        public Task<IEncrypter?> GetDecrypter(Memory<byte> bytes)
        {
            var cryptId = BitConverter.ToInt32(bytes.Slice(bytes.Length - sizeof(int), sizeof(int)).Span);
            if (cryptId == 0)
                return Task.FromResult<IEncrypter?>(null);
            var keys = Crypt.GetRsaKeysById(cryptId);
            if (keys == null)
                return Task.FromResult<IEncrypter?>(null);
            return Task.FromResult<IEncrypter?>(new RsaEncrypter(keys.PrivateServerKey));
        }
        public Task<IEncrypter?> GetEncrypter(IPacketInfo responsePacket, IPacketInfo? requestPacket)
        {
            if (requestPacket == null || IsInitPacket(requestPacket))
                return Task.FromResult<IEncrypter?>(null);

            var keys = GetKeys(requestPacket);
            if (keys == null)
                return Task.FromResult<IEncrypter?>(null);
            return Task.FromResult<IEncrypter?>(new RsaEncrypter(keys.PublicClientKey));
        }

        private RamRsa? GetKeys(IPacketInfo packet)
        {
            return Crypt.GetRsaKeysById(((ITMPacket)packet).Id);
        }
        private bool IsInitPacket(IPacketInfo packet)
        {
            if (packet is ITMPacket castedPacket && castedPacket.Id <= 0)
                return true;
            return false;
        }
    }
}
