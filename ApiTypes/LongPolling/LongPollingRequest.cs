using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.LongPolling
{
    public class LongPollingRequest : ISerializable<LongPollingRequest>
    {
        public static LongPollingRequest Deserialize(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
