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

        public static MessageHistoryResponse Deserialize(BinaryReader reader)
        {
            return new MessageHistoryResponse()
            {
                FromId = reader.ReadInt32(),
                Messages = reader.Read<Message>()
            };
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(FromId);
            writer.Write(Messages);
        }
    }
}
