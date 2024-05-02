using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Medias;

namespace ApiTypes.Communication.Messages
{
    public class MessageWithFilesSendRequest : ISerializable<MessageWithFilesSendRequest>
    {
        public SerializableFile[] Files { get; set; } = [];
        public string Text { get; set; } = string.Empty;
        public int DestinationId { get; set; }
        public MessageWithFilesSendRequest(string text, int destinationId, SerializableFile[] files)
        {
            Text = text;
            DestinationId = destinationId;
            Files = files;
        }

        public MessageWithFilesSendRequest()
        {

        }
    }
}
