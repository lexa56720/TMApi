using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Auth
{
    public class RegisterRequest : ISerializable<RegisterRequest>
    {
        public required string Username { get; init; }

        public required string Login { get; init; }

        public required string Password { get; init; }


        [SetsRequiredMembers]
        public RegisterRequest(string username, string login, string password)
        {
            Username = username;
            Login = login;
            Password = password;
        }
        public RegisterRequest()
        {

        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Username);
            writer.Write(Login);
            writer.Write(Password);
        }

        public static RegisterRequest Deserialize(BinaryReader reader)
        {
            return new RegisterRequest()
            {
                Username = reader.ReadString(),
                Login = reader.ReadString(),
                Password = reader.ReadString()
            };
        }
    }
}
