using ApiTypes.Communication.Medias;
using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Users
{
    public class User : ISerializable<User>
    {
        public string Name { get; set; }

        public int Id { get; set; }

        public string Login { get; set; }

        public bool IsOnline { get; set; } = false;

        public DateTime LastAction { get; set; }

        public Photo[] ProfilePics { get; set; } = [];

        public User(string name, int id, string login, bool isOnline, DateTime lastAction, Photo[] profilePics)
        {
            Name = name;
            Id = id;
            Login = login;
            IsOnline = isOnline;
            ProfilePics = profilePics;
            LastAction = lastAction;
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
                IsOnline = reader.ReadBoolean(),
                ProfilePics = reader.Read<Photo>()
            };
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(Id);
            writer.Write(Login);
            writer.Write(IsOnline);
            writer.Write(ProfilePics);
        }
    }
}
