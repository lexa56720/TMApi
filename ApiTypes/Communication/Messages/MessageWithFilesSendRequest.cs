namespace ApiTypes.Communication.Messages
{
    public class MessageWithFilesSendRequest : ISerializable<MessageWithFilesSendRequest>
    {
        public BaseTypes.SerializableFile[] Files { get; set; } = [];
        public string Text { get; set; } = string.Empty;
        public int DestinationId { get; set; }
        public MessageWithFilesSendRequest(string text, int destinationId, BaseTypes.SerializableFile[] files)
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
