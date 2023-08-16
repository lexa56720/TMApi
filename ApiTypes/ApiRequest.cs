using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes
{
    public class ApiRequest<T> : ISerializable<ApiRequest<T>> where T : ISerializable<T>
    {

        public string Header { get; init; } = string.Empty;
        public string Token { get; init; }

        public int Id { get; init; }

        public T Data { get; init; }


        public ApiRequest(string token, int id, T data)
        {
            Token = token;
            Data = data;
            Id = id;
        }
        public ApiRequest(string header ,string token, int id, T data)
        {
            Header = header;
            Token = token;
            Data = data;
            Id = id;
        }

        public static ApiRequest<T> Deserialize(BinaryReader reader)
        {
            return new ApiRequest<T>(reader.ReadString(),reader.ReadString(), reader.Read(),T.Deserialize(reader));
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
