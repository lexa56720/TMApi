using CSDTP;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes
{
    public class AuthData<T> : ISerializable<AuthData<T>> where T : ISerializable<T>
    {
        public required int Id { get; init; }

        public required T Data { get; init; }

        [SetsRequiredMembers]
        public AuthData(int id, T data)
        {
            Data = data;
            Id = id;
        }

        public AuthData()
        {

        }

        public static AuthData<T> Deserialize(BinaryReader reader)
        {
            return new AuthData<T>()
            {
                Id = reader.Read(),
                Data = T.Deserialize(reader)
            };
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Id);
            Data.Serialize(writer);
        }
    }
}
