using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Messages
{
    public class MessageSendRequest : ISerializable<MessageSendRequest>
    {
        public required string Text { get; init; } = string.Empty;

        public required int DestinationId { get; init; }


        [SetsRequiredMembers]
        public MessageSendRequest(string text, int destinationId)
        {
            Text = text;
            DestinationId = destinationId;
        }

        public MessageSendRequest()
        {

        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Text);
            writer.Write(DestinationId);
        }

        public static MessageSendRequest Deserialize(BinaryReader reader)
        {
            var message = new MessageSendRequest()
            {
                Text = reader.ReadString(),
                DestinationId = reader.ReadInt32(),
            };
            return message;
        }
    }
}