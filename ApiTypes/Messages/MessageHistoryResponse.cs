using CSDTP;
using CSDTP.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Messages
{
    public class MessageHistoryResponse : ISerializable<MessageHistoryResponse>
    {
        public required int FromId { get; init; }
        public required Message[] Messages { get; init; }

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
