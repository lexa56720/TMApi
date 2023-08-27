using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Auth
{
    public class AuthorizationRequest : ISerializable<AuthorizationRequest>
    {
        public required string Login { get; init; }

        public required string Password { get; init; }


        [SetsRequiredMembers]
        public AuthorizationRequest(string login, string password)
        {
            Login = login;
            Password = password;
        }
        public AuthorizationRequest()
        {

        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Login);
            writer.Write(Password);
        }

        public static AuthorizationRequest Deserialize(BinaryReader reader)
        {
            return new AuthorizationRequest()
            {
                Login = reader.ReadString(),
                Password = reader.ReadString()
            };
        }
    }
}
