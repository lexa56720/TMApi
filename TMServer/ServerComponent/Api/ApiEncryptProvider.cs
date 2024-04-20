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
        private readonly Crypts Crypt;

        public ApiEncryptProvider(Crypts crypt)
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
        public Task<IEncrypter?> GetDecrypter(Memory<byte> bytes)
        {
            var cryptId = BitConverter.ToInt32(bytes.Slice(bytes.Length - 4, 4).Span);
            var aes = Crypt.GetAesKey(cryptId);
            if (aes == null)
                return Task.FromResult<IEncrypter?>(null);
            return Task.FromResult<IEncrypter?>(new AesEncrypter((byte[])aes.AesKey.Clone()));
        }

        public Task<IEncrypter?> GetEncrypter(IPacketInfo responsePacket, IPacketInfo? requestPacket = null)
        {
            if (requestPacket is not ITMPacket reqPacket || responsePacket is not ITMPacket resPacket)
                return Task.FromResult<IEncrypter?>(null);
            resPacket.Id = reqPacket.Id;
            var aes = Crypt.GetAesKey(resPacket.Id);
            if (aes == null)
                return Task.FromResult<IEncrypter?>(null);
            return Task.FromResult<IEncrypter?>(new AesEncrypter((byte[])aes.AesKey.Clone()));
        }
    }
}
