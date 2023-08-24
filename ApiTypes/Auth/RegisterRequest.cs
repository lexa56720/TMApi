using CSDTP;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Auth
{
    public class RegisterRequest : ISerializable<RegisterRequest>
    {
        public required string Login { get; init; }

        public required string Password { get; init; }


        [SetsRequiredMembers]
        public RegisterRequest(string login, string password)
        {
            Login = login;
            Password = password;
        }
        public RegisterRequest()
        {

        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Login);
            writer.Write(Password);
        }

        public static RegisterRequest Deserialize(BinaryReader reader)
        {
            return new RegisterRequest()
            {
                Login = reader.ReadString(),
                Password = reader.ReadString()
            };
        }
    }
}
