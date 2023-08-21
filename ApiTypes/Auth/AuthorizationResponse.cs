﻿using CSDTP;
using CSDTP.Requests;
using CSDTP.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Auth
{
    public class AuthorizationResponse : ISerializable<AuthorizationResponse>
    {
        public required bool IsSuccessful { get; init; }

        public string AccessToken { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int CryptId { get; set; }

        public DateTime Expiration { get; set; }
        public byte[] AesKey { get; set; } = Array.Empty<byte>();


        public void Serialize(BinaryWriter writer)
        {
            writer.Write(IsSuccessful);
            if (IsSuccessful)
            {
                writer.WriteBytes(AesKey);

                writer.Write(UserId);
                writer.Write(CryptId);
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
                response.AesKey = reader.ReadByteArray();
                response.UserId = reader.ReadInt32();
                response.CryptId = reader.ReadInt32();
                response.AccessToken = reader.ReadString();
                response.Expiration = DateTime.FromBinary(reader.ReadInt64());
            }

            return response;
        }
    }
}
