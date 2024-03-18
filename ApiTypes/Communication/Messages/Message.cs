using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Messages
{
    public class Message : ISerializable<Message>
    {
        public int Id { get; set; }

        public int AuthorId { get; set; }

        public int DestinationId { get; set; }

        public string Text { get; set; }

        public DateTime SendTime { get; set; }

        public bool IsReaded { get; set; }

        public bool IsSystem { get; set; }

        public Message()
        {

        }

        public Message(int id, int authorId, int destinationId, string text, DateTime sendTime,bool isReaded,bool isSystem)
        {
            Id = id;
            AuthorId = authorId;
            DestinationId = destinationId;
            Text = text;
            SendTime = sendTime;
            IsReaded = isReaded;
            IsSystem = isSystem;
        }
    }
}
