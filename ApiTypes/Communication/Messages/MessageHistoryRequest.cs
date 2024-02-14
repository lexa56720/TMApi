using ApiTypes.Communication.Chats;
using CSDTP;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Messages
{
    public class MessageHistoryRequest :ISerializable<MessageHistoryRequest>
    {
        public int ChatId { get; set; }
        public int MaxCount { get; set; } = 20;
        public int LastMessageId { get; set; }

        [SetsRequiredMembers]
        public MessageHistoryRequest(int chatId,int lastMessageId,int maxCount=20)
        {
            ChatId = chatId;
            LastMessageId = lastMessageId;
            MaxCount = maxCount;
        }
        public MessageHistoryRequest()
        {

        }

    }
}
