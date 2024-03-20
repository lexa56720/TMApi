using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Chats
{
    public class InviteRequest:ISerializable<InviteRequest>
    {
        public int[] Ids { get; set; } = [];

        public InviteRequest(int[] ids)
        {
            Ids = ids;
        }

        public InviteRequest()
        {
        }
    }
}
