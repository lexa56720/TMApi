using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Users
{
    public class User : ISerializable<User>
    {
        public required string Name { get; init; }

        public required int Id { get; init; }

        public required string Login { get; init; }

        public required bool IsOnline { get; init; } = false;

        [SetsRequiredMembers]
        public User(string name, int id,string login, bool isOnline)
        {
            Name = name;
            Id = id;
            Login = login;
            IsOnline = isOnline;
        }
        public User()
        {

        }

        public static User Deserialize(BinaryReader reader)
        {
            return new User()
            {
                Name = reader.ReadString(),
                Id = reader.ReadInt32(),
                Login = reader.ReadString(),
                IsOnline = reader.ReadBoolean()
            };
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(Id);
            writer.Write(Login);
            writer.Write(IsOnline);
        }
    }
}
