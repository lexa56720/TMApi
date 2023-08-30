using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Info
{
    public class VersionRequest : ISerializable<VersionRequest>
    {
        public static VersionRequest Deserialize(BinaryReader reader)
        {
            return new VersionRequest();
        }

        public void Serialize(BinaryWriter writer)
        {
            return;
        }
    }
}
