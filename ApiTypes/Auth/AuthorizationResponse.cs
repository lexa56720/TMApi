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
        public required bool IsSuccessful { get;  init; }

        public string AccessToken { get; set; } = string.Empty;
        public int Id { get; set; }
        public DateTime Expiration { get; set; }
        public byte[] AesKey { get; set; } = Array.Empty<byte>();


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

        public static AuthorizationResponse Deserialize(BinaryReader reader)
        {
            var response = new AuthorizationResponse()
            {
                IsSuccessful = reader.ReadBoolean()
            };
            if (response.IsSuccessful)
            {
                response.AesKey = reader.ReadBytes(reader.ReadInt32());
                response.Id = reader.ReadInt32();
                response.AccessToken = reader.ReadString();
                response.Expiration = DateTime.FromBinary(reader.ReadInt64());
            }

            return response;
        }
    }
}
