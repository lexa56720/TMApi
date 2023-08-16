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
        public string Token { get; init; }

        public T Data { get; init; }


        public ApiRequest(string token,T data)
        {
            Token = token;
            Data = data;
        }

        public static ApiRequest<T> Deserialize(BinaryReader reader)
        {
            return new ApiRequest<T>(reader.ReadString(), T.Deserialize(reader));
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Token);
            Data.Serialize(writer);
        }
    }
}
