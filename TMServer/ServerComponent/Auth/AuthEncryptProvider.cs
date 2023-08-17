using CSDTP.Cryptography.Algorithms;
using CSDTP.Cryptography.Providers;
using CSDTP.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase;

namespace TMServer.ServerComponent.Auth
{
    internal class AuthEncryptProvider : IEncryptProvider
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void DisposeEncryptor(IEncrypter encryptor)
        {
            encryptor.Dispose();
        }

        public IEncrypter? GetDecrypter(IPacketInfo packet)
        {
            var keys = GetKeys(packet);
            if (keys == null)
                return null;
            var rsaDecrypter = new RsaEncrypter(keys.ServerPrivateKey);
            return rsaDecrypter;
        }
        public IEncrypter? GetEncrypter(IPacketInfo packet)
        {
            var keys = GetKeys(packet);
            if (keys == null)
                return null;
            var rsaDecrypter = new RsaEncrypter(keys.ClientPublicKey);
            return rsaDecrypter;
        }

        private KeyPair? GetKeys(IPacketInfo packet)
        {
            return Security.GetRsaKeysByIp(BitConverter.ToUInt32(packet.Source.GetAddressBytes()));
        }

    }
}
