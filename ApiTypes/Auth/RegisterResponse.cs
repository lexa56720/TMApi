using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Auth
{
    public class RegisterResponse:ISerializable<RegisterResponse>
    {
        public required bool IsSuccessful { get; init; }

        public RegisterResponse()        
        { 

        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(IsSuccessful);
        }

        public static RegisterResponse Deserialize(BinaryReader reader)
        {
            return new RegisterResponse()
            {
               IsSuccessful=reader.ReadBoolean(),
            };
        }
    }
}
