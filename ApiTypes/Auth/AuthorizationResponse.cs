using CSDTP;
using CSDTP.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Auth
{
    public class AuthorizationResponse : ISerializable<AuthorizationResponse>
    {

        public bool IsSuccessful { get; private set; }

        public string AccessToken { get; private set; } = string.Empty;
        public int Id { get; private set; }
        public DateTime Expiration { get; private set; }

        public byte[] AesKey { get; private set; } = Array.Empty<byte>();


        public static AuthorizationResponse Deserialize(BinaryReader reader)
        {
            var response = new AuthorizationResponse();
            response.IsSuccessful = reader.ReadBoolean();
            if (response.IsSuccessful)
            {
                response.AesKey = reader.ReadBytes(reader.ReadInt32());
                response.Id = reader.ReadInt32();
                response.AccessToken = reader.ReadString();
                response.Expiration = DateTime.FromBinary(reader.ReadInt64());
            }

            return response;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(IsSuccessful);
            if (IsSuccessful)
            {
                writer.Write(AesKey.Length);
                writer.Write(AesKey);

                writer.Write(Id);
                writer.Write(AccessToken);
                writer.Write(Expiration.ToBinary());
            }

        }
    }
}
