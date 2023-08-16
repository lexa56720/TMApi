using ApiTypes.Messages;
using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes
{
    public class MessageHistory : ISerializable<MessageHistory>
    {
        public Message[] Messages { get; private set; } = Array.Empty<Message>();

        public int Count { get; private set; }
        public MessageHistory(Message[] messages)
        {
            Messages = messages;
            Count = Messages.Length;
        }
        public MessageHistory()
        {
        }

        public static MessageHistory Deserialize(BinaryReader reader)
        {
            var history = new MessageHistory()
            {
                Count = reader.ReadInt32()
            };

            history.Messages = new Message[history.Count];

            for (int i = 0; i < history.Count; i++)
                history.Messages[i]=Message.Deserialize(reader);

            return history;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Count);

            for(int i = 0; i < Count; i++)
                Messages[i].Serialize(writer);
        }
    }
}
