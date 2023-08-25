using CSDTP;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Messages
{
    public class MessageHistoryRequest : ISerializable<MessageHistoryRequest>
    {
        public required int FromId { get; init; }

        public int Offset { get; init; } = 0;

        public int MaxCount { get; init; } = 20;

        [SetsRequiredMembers]
        public MessageHistoryRequest(int fromId,int offset=0,int maxCount=20)
        {
            FromId = fromId;
            Offset = offset;
            MaxCount = maxCount;
        }
        public MessageHistoryRequest()
        {
        }

        public static MessageHistoryRequest Deserialize(BinaryReader reader)
        {
            return new MessageHistoryRequest()
            {
                FromId = reader.ReadInt32(),
                Offset = reader.ReadInt32(),
                MaxCount = reader.ReadInt32(),
            };
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(FromId);
            writer.Write(Offset);
            writer.Write(MaxCount);
        }
    }
}
