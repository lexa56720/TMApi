using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Messages
{
    public class LastMessagesRequest : ISerializable<LastMessagesRequest>
    {
        public required int ChatId { get; init; }

        public int Offset { get; init; } = 0;

        public int MaxCount { get; init; } = 20;

        [SetsRequiredMembers]
        public LastMessagesRequest(int fromId, int offset = 0, int maxCount = 20)
        {
            ChatId = fromId;
            Offset = offset;
            MaxCount = maxCount;
        }
        public LastMessagesRequest()
        {
        }
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(ChatId);
            writer.Write(Offset);
            writer.Write(MaxCount);
        }
        public static LastMessagesRequest Deserialize(BinaryReader reader)
        {
            return new LastMessagesRequest()
            {
                ChatId = reader.ReadInt32(),
                Offset = reader.ReadInt32(),
                MaxCount = reader.ReadInt32(),

            };
        }
    }
}
