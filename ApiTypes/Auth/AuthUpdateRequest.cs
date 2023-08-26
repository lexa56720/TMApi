using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Auth
{
    public  class AuthUpdateRequest : ISerializable<AuthUpdateRequest>
    {
        public static AuthUpdateRequest Deserialize(BinaryReader reader)
        {
            return new AuthUpdateRequest();
        }

        public void Serialize(BinaryWriter writer)
        {
            return;
        }
    }
}
