using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Chats
{
    public class ChangeCoverRequest:ISerializable<ChangeCoverRequest>
    {
        public int ChatId { get; set; }

        public byte[] NewCover { get; set; } = [];

        public ChangeCoverRequest(int chatId, byte[] imageData)
        {
            ChatId = chatId;
            NewCover = imageData;
        }
        public ChangeCoverRequest()
        {

        }
    }
}
