using ApiTypes.Users;
using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Messages
{
    public class Message : ISerializable<Message>
    {
        public required string Text { get; init; } = string.Empty;


        public required bool IsHasAuthor { get; init; }
        public User? Author { get; private set; }

        public int Id { get; init; } = -1;

        public required int DestinationId { get; init; }

        public bool IsHasTime { get; init; }

        public DateTime SendTime { get; private set; }


        [SetsRequiredMembers]
        public Message(string text, int destinationId)
        {
            Text = text;
            DestinationId = destinationId;
            IsHasAuthor = false;
            IsHasTime = false;
        }

        [SetsRequiredMembers]
        public Message(string text, User author, int id, DateTime sendTime)
        {
            Text = text;
            Author = author;
            Id = id;
            SendTime = sendTime;

            IsHasAuthor = true;
            IsHasTime = true;
        }

        public Message()
        {

        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Text);
            writer.Write(DestinationId);

            writer.Write(IsHasTime);
            writer.Write(IsHasAuthor);

            writer.Write(Id);

            writer.Write(IsHasTime);
            if (IsHasTime)
                writer.Write(SendTime.ToBinary());

            writer.Write(IsHasAuthor);
            if (IsHasAuthor)
                Author.Serialize(writer);
        }

        public static Message Deserialize(BinaryReader reader)
        {
            var message = new Message()
            {
                Text= reader.ReadString(),
                DestinationId= reader.ReadInt32(),
                IsHasTime = reader.ReadBoolean(),
                IsHasAuthor = reader.ReadBoolean(),
                Id = reader.ReadInt32(),
            };

            if (message.IsHasTime)
                message.SendTime = DateTime.FromBinary(reader.ReadInt64());
            if (message.IsHasAuthor)
                message.Author = User.Deserialize(reader);

            return message;
        }
    }
}