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

        public  DateTime SendTime { get; set; }


        public Message()
        {

        }

        [SetsRequiredMembers]
        public Message(int id, int authorId, int destinationId, string text, DateTime sendTime)
        {
            Id = id;
            AuthorId = authorId;
            DestinationId = destinationId;
            Text = text;
            SendTime = sendTime;
     
        }
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(AuthorId);
            writer.Write(DestinationId);
            writer.Write(Text);
            writer.Write(SendTime.ToBinary());
        }

        public static Message Deserialize(BinaryReader reader)
        {
            return new Message()
            {
                Id = reader.ReadInt32(),
                AuthorId = reader.ReadInt32(),
                DestinationId = reader.ReadInt32(),
                Text = reader.ReadString(),
                SendTime = DateTime.FromBinary(reader.ReadInt64())
            };
        }
    }
}
