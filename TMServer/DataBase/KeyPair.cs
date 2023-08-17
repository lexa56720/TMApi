using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase
{
    public class KeyPair
    {
        public string ServerPrivateKey { get; init; }
        public string ClientPublicKey { get; init; }
    }

}
