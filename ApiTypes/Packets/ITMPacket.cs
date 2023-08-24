using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Packets
{
    public interface ITMPacket
    {
        public IdHolder Id { get; }
    }
}
