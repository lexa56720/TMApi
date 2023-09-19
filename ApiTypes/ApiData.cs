using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes
{
    public class ApiData<T> : ISerializable<ApiData<T>> where T : ISerializable<T>
    {
        public RequestHeaders Header { get; init; } = RequestHeaders.None;
        public required string Token { get; init; }

        public required int UserId { get; init; }

        public required int CryptId { get; init; }

        public required T Data { get; init; }

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
