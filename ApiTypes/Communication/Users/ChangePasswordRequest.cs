using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Users
{
    public class ChangePasswordRequest:ISerializable<ChangePasswordRequest>
    {
        public string CurrentPasswordHash { get; set; } = string.Empty;

        public string NewPasswordHash { get; set; } = string.Empty;

        public ChangePasswordRequest() { }
    }
}
