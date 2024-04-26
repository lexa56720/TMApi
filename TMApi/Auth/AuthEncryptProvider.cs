using ApiTypes.Communication.Packets;
using CSDTP.Cryptography.Algorithms;
using CSDTP.Cryptography.Providers;
using CSDTP.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMApi.Auth
{
    internal class AuthEncryptProvider : IEncryptProvider
    {
        private bool IsDisposed;
        private readonly int CryptId;

        public IEncrypter Encrypter { get; }

        public IEncrypter Decrypter { get; }

        public AuthEncryptProvider(IEncrypter encrypter, int cryptId) : this(encrypter, encrypter, cryptId)
        {
        }

        public AuthEncryptProvider(IEncrypter encrypter, IEncrypter decrypter, int cryptId)
        {
            Encrypter = encrypter;
            Decrypter = decrypter;
            CryptId = cryptId;
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            if (Encrypter != Decrypter)
            {
                Encrypter.Dispose();
                Decrypter.Dispose();
            }
            else
                Encrypter.Dispose();


            IsDisposed = true;
        }

        public void DisposeEncrypter(IEncrypter encryptor)
        {
        }

        public Task<IEncrypter?> GetDecrypter(Memory<byte> bytes)
        {
            return Task.FromResult<IEncrypter?>(Decrypter);
        }

        public Task<IEncrypter?> GetEncrypter(IPacketInfo responsePacket, IPacketInfo? requestPacket = null)
        {
            if (responsePacket is not ITMPacket packet)
                throw new Exception("WRONG PACKET TYPE");

            packet.Id = CryptId;
            return Task.FromResult<IEncrypter?>(Encrypter);
        }
    }
}
