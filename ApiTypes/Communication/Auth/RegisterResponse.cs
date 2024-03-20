using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Auth
{
    public class RegisterResponse : ISerializable<RegisterResponse>
    {
        public bool IsAccepted { get; set; }

        public int RequestId { get; set; } = -1;

        public RegisterResponse(bool isAccepted)
        {
            IsAccepted = isAccepted;
        }

        public RegisterResponse()
        {
        }
    }
}

