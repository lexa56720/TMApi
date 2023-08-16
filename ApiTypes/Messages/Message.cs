using CSDTP;

namespace ApiTypes.Messages
{
    public class Message : ISerializable<Message>
    {
        public string Text { get; private set; } = string.Empty;

        public User? Author { get; private set; }

        public int Id { get; private set; } = -1;

        public int DestinationId { get; private set; }


        public bool IsHasAuthor { get; private set; }

        public bool IsHasTime { get; private set; }

        public bool IsHasId { get; private set; }

        public DateTime SendTime { get; private set; }


        public Message(string text, int destinationId)
        {
            Text = text;
            DestinationId = destinationId;
            IsHasAuthor = false;
            IsHasTime = false;
            IsHasId = false;
        }

        public Message(string text, User author, int id, DateTime sendTime)
        {
            Text = text;
            Author = author;
            Id = id;
            SendTime = sendTime;

            IsHasAuthor = true;
            IsHasTime = true;
            IsHasId = true;
        }

        public static Message Deserialize(BinaryReader reader)
        {
            var message = new Message(reader.ReadString(), reader.Read())
            {
                IsHasId = reader.ReadBoolean(),
                IsHasTime = reader.ReadBoolean(),
                IsHasAuthor = reader.ReadBoolean(),
            };

            if (message.IsHasId)
                message.Id = reader.ReadInt32();
            if (message.IsHasTime)
                message.SendTime = DateTime.FromBinary(reader.ReadInt64());
            if (message.IsHasAuthor)
                message.Author = User.Deserialize(reader);

            return message;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Text);
            writer.Write(DestinationId);

            writer.Write(IsHasId);
            writer.Write(IsHasTime);
            writer.Write(IsHasAuthor);

            if (IsHasId)
                writer.Write(Id);

            writer.Write(IsHasTime);
            if (IsHasTime)
                writer.Write(SendTime.ToBinary());


            writer.Write(IsHasAuthor);
            if (IsHasAuthor)
                Author.Serialize(writer);
        }
    }
}