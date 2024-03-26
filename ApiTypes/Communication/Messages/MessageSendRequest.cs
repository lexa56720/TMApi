using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Messages
{
    public class MessageSendRequest : ISerializable<MessageSendRequest>
    {
        public string Text { get; set; } = string.Empty;

        public int DestinationId { get; set; }

        public MessageSendRequest(string text, int destinationId)
        {
            Text = text;
            DestinationId = destinationId;
        }

        public MessageSendRequest()
        {

        }
    }
}