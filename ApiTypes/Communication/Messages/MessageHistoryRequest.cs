using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Messages
{
    public class MessageHistoryRequest : ISerializable<MessageHistoryRequest>
    {
        public required int ChatId { get; init; }

        public int LastMessageId { get; init; } = -1;

        public DateTime LastMessageDate { get; init; } = DateTime.MinValue;

        public int Offset { get; init; } = 0;

        public int MaxCount { get; init; } = 20;

        [SetsRequiredMembers]
        public MessageHistoryRequest(int fromId, int offset = 0, int maxCount = 20)
        {
            ChatId = fromId;
            Offset = offset;
            MaxCount = maxCount;
        }
        [SetsRequiredMembers]
        public MessageHistoryRequest(Message lastMessage, int fromId, int offset = 0, int maxCount = 20)
        {
            ChatId = fromId;
            Offset = offset;
            MaxCount = maxCount;
            LastMessageId = lastMessage.Id;
            LastMessageDate = lastMessage.SendTime;
        }
        public MessageHistoryRequest()
        {
        }
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(ChatId);
            writer.Write(Offset);
            writer.Write(MaxCount);
            writer.Write(LastMessageId);
            writer.Write(LastMessageDate.ToBinary());
        }
        public static MessageHistoryRequest Deserialize(BinaryReader reader)
        {
            return new MessageHistoryRequest()
            {
                ChatId = reader.ReadInt32(),
                Offset = reader.ReadInt32(),
                MaxCount = reader.ReadInt32(),
                LastMessageId = reader.ReadInt32(),
                LastMessageDate = DateTime.FromBinary(reader.ReadInt64())
            };
        }
    }
}
