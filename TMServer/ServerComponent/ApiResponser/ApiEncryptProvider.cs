using ApiTypes.Packets;
using CSDTP.Cryptography.Algorithms;
using CSDTP.Cryptography.Providers;
using CSDTP.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase;

namespace TMServer.ServerComponent.ApiResponser
{
    internal class ApiEncryptProvider : IEncryptProvider
    {
        public void Dispose()
        {
            return;
        }

        public void DisposeEncryptor(IEncrypter encryptor)
        {
            encryptor.Dispose();
        }

        public IEncrypter? GetDecrypter(IPacketInfo packet)
        {
            var cryptId = ((ITMPacket)packet).Id.InstanceValue;
            var key= Security.GetAesKey(cryptId);
            return new AesEncrypter(key);
        }

        public IEncrypter? GetEncrypter(IPacketInfo packet)
        {
            var cryptId = ((ITMPacket)packet.InfoObj).Id.InstanceValue;
            var key = Security.GetAesKey(cryptId);
            return new AesEncrypter(key);
        }
    }
}
