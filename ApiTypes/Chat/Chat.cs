using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes
{
    public class Chat : ISerializable<Chat>
    {
        public User[] Users { get; private set; } = Array.Empty<User>();

        public int MemberCount { get; private set; }

        public User Admin { get; private set; }

        public int Id { get; private set; }

        public int TotalMessages { get; private set; }

        public Chat(int id, User admin, params User[] users)
        {
            Id = id;
            Admin = admin;
            Users = users;
        }

        public Chat()
        {
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(MemberCount);
            writer.Write(TotalMessages);
            Admin.Serialize(writer);

            for (int i = 0; i < Users.Length; i++)
                Users[i].Serialize(writer);
        }

        public static Chat Deserialize(BinaryReader reader)
        {
            var chat = new Chat()
            {
                Id = reader.ReadInt32(),
                MemberCount = reader.ReadInt32(),
                TotalMessages = reader.ReadInt32(),
                Admin = User.Deserialize(reader),
            };

            chat.Users=new User[chat.MemberCount];
            for (int i = 0; i < chat.MemberCount; i++)
                chat.Users[i] = User.Deserialize(reader);

            return chat;
        }
    }
}
