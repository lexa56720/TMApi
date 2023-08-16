using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes
{
    public class UnauthorizedRequest<T> : ISerializable<UnauthorizedRequest<T>> where T : ISerializable<T>
    {
        public int Id { get; }
        public T Data { get; set; }

        public UnauthorizedRequest(T data) 
        {
            Data = data;
        }
        public UnauthorizedRequest(T data,int id)
        {
            Data = data;
            Id = id;
        }
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Id);
            Data.Serialize(writer);
        }

        public static UnauthorizedRequest<T> Deserialize(BinaryReader reader)
        {
            return new UnauthorizedRequest<T>(T.Deserialize(reader), reader.ReadInt32());
        }
    }
}
