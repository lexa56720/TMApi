using CSDTP.Cryptography.Algorithms;
using CSDTP.Cryptography.Providers;
using CSDTP.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.ServerComponent.ApiResponser
{
    internal class ApiEncryptProvider : IEncryptProvider
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void DisposeEncryptor(IEncrypter encryptor)
        {
            throw new NotImplementedException();
        }

        public IEncrypter? GetDecrypter(IPacketInfo packet)
        {
            throw new NotImplementedException();
        }

        public IEncrypter? GetEncrypter(IPacketInfo packet)
        {
            throw new NotImplementedException();
        }
    }
}
