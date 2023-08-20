using CSDTP;
using CSDTP.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.BaseTypes
{
    public class SerializableArray<T> : ISerializable<SerializableArray<T>> where T : ISerializable<T>
    {
        public T[] Items { get; set; }

        public SerializableArray(params T[] items)
        {
            Items = items;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Items);
        }

        public static SerializableArray<T> Deserialize(BinaryReader reader)
        {
            return new SerializableArray<T>(reader.Read<T>());
        }
    }
}
