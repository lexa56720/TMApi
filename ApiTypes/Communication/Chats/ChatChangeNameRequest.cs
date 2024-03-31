using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Chats
{
    public class ChatChangeNameRequest : ISerializable<ChatChangeNameRequest>
    {
        public string NewName { get; set; }
        public int ChatId { get; set; }

        public ChatChangeNameRequest()
        {

        }

        [SetsRequiredMembers]
        public ChatChangeNameRequest(string newName,int chatId)
        {
            NewName = newName;
            ChatId = chatId;
        }
    }
}
