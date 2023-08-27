using ApiTypes.Communication.Users;
using CSDTP;
using CSDTP.Utils;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Chats
{
    public class Chat : ISerializable<Chat>
    {
        public required User[] Users { get; init; } = Array.Empty<User>();

        public required User Admin { get; init; }

        public required int Id { get; init; }

        public required int TotalMessages { get; init; }

        [SetsRequiredMembers]
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
            writer.Write(TotalMessages);
            Admin.Serialize(writer);

            writer.Write(Users);
        }

        public static Chat Deserialize(BinaryReader reader)
        {
            var chat = new Chat()
            {
                Id = reader.ReadInt32(),
                TotalMessages = reader.ReadInt32(),
                Admin = User.Deserialize(reader),
                Users = reader.Read<User>(),
            };
            return chat;
        }
    }
}
