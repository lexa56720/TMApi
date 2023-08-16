using CSDTP;
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

        public int Count { get; set; }


        public SerializableArray(params T[] items)
        {
            Items = items;
            Count = items.Length;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Items.Length);
            for (int i = 0; i < Items.Length; i++)
                Items[i].Serialize(writer);
        }

        public static SerializableArray<T> Deserialize(BinaryReader reader)
        {
            var length = reader.ReadInt32();
            var items = new T[length];

            for (int i = 0; i < length; i++)
                items[i] = T.Deserialize(reader);
            return new SerializableArray<T>(items);
        }
    }
}
