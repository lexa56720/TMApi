using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Auth
{
    public class AuthorizationRequest : ISerializable<AuthorizationRequest>
    {

        public string Login { get; private set; }

        public string Password { get; private set; }

        public AuthorizationRequest(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public static AuthorizationRequest Deserialize(BinaryReader reader)
        {
            return new AuthorizationRequest(reader.ReadString(), reader.ReadString());
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Login);
            writer.Write(Password);
        }
    }
}
