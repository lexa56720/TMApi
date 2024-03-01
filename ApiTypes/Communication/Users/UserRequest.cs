using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Users
{
   public class UserRequest:ISerializable<UserRequest>
    {
        public required int[] Ids { get; set; }

        [SetsRequiredMembers]
        public UserRequest(int[] ids)
        {
            Ids = ids;
        }

        [SetsRequiredMembers]
        public UserRequest()
        {
        }
    }
}
