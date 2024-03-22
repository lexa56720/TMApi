using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Chats
{
    public class ChatLeaveRequest : ISerializable<ChatLeaveRequest>
    {
        public int ChatId { get; set; }

        public ChatLeaveRequest(int chatId)
        {
            ChatId = chatId;
        }
        public ChatLeaveRequest()
        {

        }
    }
}
