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

        public Photo[] ProfilePics { get; set; } = [];

        public User(string name, int id, string login, bool isOnline, Photo[] profilePics)
        {
            Name = name;
            Id = id;
            Login = login;
            IsOnline = isOnline;
            ProfilePics = profilePics;
        }
        public User()
        {

        }
    }
}
