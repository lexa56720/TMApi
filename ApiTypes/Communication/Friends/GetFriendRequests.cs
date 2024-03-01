using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Friends
{
    public class GetFriendRequests:ISerializable<GetFriendRequests>
    {
        public required int[] Ids { get; set; }

        [SetsRequiredMembers]
        public GetFriendRequests(int[] ids)
        {
            Ids = ids;
        }

        [SetsRequiredMembers]
        public GetFriendRequests()
        {
        }
    }
}
