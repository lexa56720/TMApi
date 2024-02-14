using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Auth
{
    public class AuthorizationRequest : ISerializable<AuthorizationRequest>
    {
        public string Login { get; set; }

        public string Password { get; set; }


        [SetsRequiredMembers]
        public AuthorizationRequest(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public AuthorizationRequest()
        {

        }

    }
}
