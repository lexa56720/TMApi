using CSDTP;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes
{
    public class ApiData<T> : ISerializable<ApiData<T>> where T : ISerializable<T>
    {

        public string Header { get; init; } = string.Empty;
        public required string Token { get; init; }

        public required int Id { get; init; }

        public required T Data { get; init; }

        [SetsRequiredMembers]
        public ApiData(string token, int id, T data)
        {
            Token = token;
            Data = data;
            Id = id;
        }
        [SetsRequiredMembers]
        public ApiData(string header ,string token, int id, T data)
        {
            Header = header;
            Token = token;
            Data = data;
            Id = id;
        }

        public ApiData()
        {

        }

        public static ApiData<T> Deserialize(BinaryReader reader)
        {
            return new ApiData<T>()
            {
                Header = reader.ReadString(),
                Token = reader.ReadString(),
                Id = reader.Read(),
                Data = T.Deserialize(reader)
            };
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Header);
            writer.Write(Token); 
            writer.Write(Id);
            Data.Serialize(writer);     
        }
    }
}
