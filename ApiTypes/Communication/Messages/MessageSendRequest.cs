using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Messages
{
    public class MessageSendRequest : BaseMessageSendRequest, ISerializable<MessageSendRequest>
    {
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