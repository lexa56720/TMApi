using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Types
{
    internal class AesData
    {
        public required byte[] Key { get; init; }

        public required byte[] IV { get; init; }
    }
}
