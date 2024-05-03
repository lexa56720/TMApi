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
            //Получение идентификатора шифрования
            var cryptId = BitConverter.ToInt32(bytes.Slice(bytes.Length - sizeof(int), sizeof(int)).Span);
            if (cryptId == 0)
                return Task.FromResult<IEncrypter?>(null);
            //Получение ключей шифрования
            var keys = Crypt.GetRsaKeysById(cryptId);
            if (keys == null)
                return Task.FromResult<IEncrypter?>(null);
            //Возврат объекта готового выполнить дешифрование по алгоритму RSA
            return Task.FromResult<IEncrypter?>(new RsaEncrypter(keys.PrivateServerKey));
        }
        public Task<IEncrypter?> GetEncrypter(IPacketInfo responsePacket, IPacketInfo? requestPacket)
        {
            //Если запрос является иницизирующим, то шифрование не требуется
            if (requestPacket == null || IsInitPacket(requestPacket))
                return Task.FromResult<IEncrypter?>(null);

            //Получение ключей
            var keys = GetKeys(requestPacket);
            if (keys == null)
                return Task.FromResult<IEncrypter?>(null);

            //Возврат объекта готового выполнить шифрование по алгоритму RSA
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
