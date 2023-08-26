using CSDTP;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Messages
{
    public class Message : ISerializable<Message>
    {
        public required int Id { get; init; }

        public required int AuthorId { get; init; }

        public required string Text { get; init; }

        public required DateTime SendTime { get; init; }


        public Message()
        {

        }

        [SetsRequiredMembers]
        public Message(int id,int authorId,string text,DateTime sendTime)
        {
            Id = id;
            AuthorId = authorId;
            Text = text;
            SendTime = sendTime;
        }
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(AuthorId);
            writer.Write(Text);
            writer.Write(SendTime.ToBinary());
        }

        public static Message Deserialize(BinaryReader reader)
        {
            return new Message()
            {
                Id = reader.ReadInt32(),
                AuthorId = reader.ReadInt32(),
                Text = reader.ReadString(),
                SendTime = DateTime.FromBinary(reader.ReadInt64())
            };
        }
    }
}
