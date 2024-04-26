using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Auth
{
    public class RegistrationResponse : ISerializable<RegistrationResponse>
    {
        public bool IsAccepted { get; set; }

        public int RequestId { get; set; } = -1;

        public RegistrationResponse(bool isAccepted)
        {
            IsAccepted = isAccepted;
        }

        public RegistrationResponse()
        {
        }
    }
}

