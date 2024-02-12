using CSDTP;
using AutoSerializer;
using System.Diagnostics.CodeAnalysis;
namespace ApiTypes
{
    public class ApiData<T> : ISerializable<ApiData<T>> where T : ISerializable<T>,new()
    {
        public RequestHeaders Header { get; set; } = RequestHeaders.None;
        public required string Token { get; set; }

        public required int UserId { get; set; }

        public required int CryptId { get; set; }

        public required T Data { get; set; }

        [SetsRequiredMembers]
        public ApiData(string token, int id, T data)
        {
            Token = token;
            Data = data;
            UserId = id;
        }
        [SetsRequiredMembers]
        public ApiData(RequestHeaders header, string token, int id, T data, int cryptId)
        {
            Header = header;
            Token = token;
            Data = data;
            UserId = id;
            CryptId = cryptId;
        }
        [SetsRequiredMembers]
        public ApiData()
        {

        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write((byte)Header);
            writer.Write(Token);
            writer.Write(UserId);
            writer.Write(CryptId);
            Data.Serialize(writer);
        }
        public static ApiData<T> Deserialize(BinaryReader reader)
        {
            return new ApiData<T>()
            {
                Header = (RequestHeaders)reader.ReadByte(),
                Token = reader.ReadString(),
                UserId = reader.ReadInt32(),
                CryptId = reader.ReadInt32(),
                Data = T.Deserialize(reader)
            };
        }

    }
}
