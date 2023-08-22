using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase
{
    public class KeyPair
    {
        public required string ServerPrivateKey { get; init; }
        public required string ClientPublicKey { get; init; }
    }

}
