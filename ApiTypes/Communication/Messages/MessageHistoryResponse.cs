using CSDTP;
using CSDTP.Utils;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Messages
{
    public class MessageHistoryResponse : ISerializable<MessageHistoryResponse>
    {
        public int FromId { get; set; }
        public Message[] Messages { get; set; }

        [SetsRequiredMembers]
        public MessageHistoryResponse(int fromId, Message[] messages)
        {
            FromId = fromId;
            Messages = messages;
        }

        public MessageHistoryResponse()
        {
        }

    }
}
