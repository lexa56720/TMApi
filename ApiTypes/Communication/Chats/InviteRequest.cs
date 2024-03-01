using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Chats
{
    internal class InviteRequest
    {
        public int[] Ids { get; set; }


        [SetsRequiredMembers]
        public InviteRequest(int[] ids)
        {
            Ids = ids;
        }

        public InviteRequest()
        {
        }
    }
}
