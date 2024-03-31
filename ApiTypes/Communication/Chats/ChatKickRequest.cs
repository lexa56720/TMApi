using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Chats
{
    public class ChatKickRequest:ISerializable<ChatKickRequest>
    {
        public int ChatId { get; set; }
        
        public int UserId { get; set; }
        public ChatKickRequest() 
        { 
        }

        public ChatKickRequest(int chatId,int userId)
        {
            ChatId = chatId;
            UserId = userId;
        }
    }
}
