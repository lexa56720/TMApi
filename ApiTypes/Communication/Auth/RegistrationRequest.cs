using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Auth
{
    public class RegistrationRequest : ISerializable<RegistrationRequest>
    {
        public string Username { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public RegistrationRequest(string username, string login, string password)
        {
            Username = username;
            Login = login;
            Password = password;
        }
        public RegistrationRequest()
        {

        }

    }
}
