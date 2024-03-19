using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Chats
{
    public class InviteToChatRequest : ISerializable<InviteToChatRequest>
    {
        public int[] UserIds { get; set; }

        public int ChatId { get; set; }

        public InviteToChatRequest() { }

        public InviteToChatRequest(int chatId, int[] usersIds) 
        {
            UserIds = usersIds;
            ChatId = chatId;
        }
    }
}
