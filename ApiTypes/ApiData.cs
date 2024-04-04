using CSDTP;
using AutoSerializer;
using System.Diagnostics.CodeAnalysis;
namespace ApiTypes
{
    public class ApiData<T> :IApiData, ISerializable<ApiData<T>> where T : ISerializable<T>,new()
    {
        public required string Token { get; set; }
        public required int UserId { get; set; }
        public required T Data { get; set; }
        public int CryptId { get; set; } = -1;

        [SetsRequiredMembers]
        public ApiData(string token, int id, T data)
        {
            Token = token;
            Data = data;
            UserId = id;
        }
        [SetsRequiredMembers]
        public ApiData(string token, int id, T data, int cryptId)
        {
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
            writer.Write(Token);
            writer.Write(UserId);
            writer.Write(CryptId);
            Data.Serialize(writer);
        }
        public static ApiData<T> Deserialize(BinaryReader reader)
        {
            return new ApiData<T>()
            {
                Token = reader.ReadString(),
                UserId = reader.ReadInt32(),
                CryptId = reader.ReadInt32(),
                Data = T.Deserialize(reader)
            };
        }

    }
}
