using ApiTypes.Auth;
using ApiTypes.Packets;
using CSDTP.Cryptography.Algorithms;
using CSDTP.Cryptography.Providers;
using CSDTP.Packets;
using CSDTP.Requests.RequestHeaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase;
using TMServer.DataBase.Tables;
using TMServer.DataBase.Types;

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

        private RsaCrypt GetKeys(IPacketInfo packet)
        {
            var rsa = Security.GetRsaKeysById(((ITMPacket)packet).Id.InstanceValue);
            ArgumentNullException.ThrowIfNull(rsa);
            return rsa;
        }
        private bool IsInitPacket(IPacketInfo packet)
        {
            if (((ITMPacket)(IPacketInfo)packet.InfoObj).Id.InstanceValue <= 0)
                return true;
            return false;
        }
    }
}
