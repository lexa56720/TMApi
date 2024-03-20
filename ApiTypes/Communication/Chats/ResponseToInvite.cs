using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Chats
{
    public class ResponseToInvite:ISerializable<ResponseToInvite>
    {
        public int InviteId { get; set; }
        public bool IsAccepted { get; set; }

        public ResponseToInvite() { }

        public ResponseToInvite(int inviteId, bool isAccepted)
        {
            InviteId = inviteId;
            IsAccepted = isAccepted;
        }
    }
}
