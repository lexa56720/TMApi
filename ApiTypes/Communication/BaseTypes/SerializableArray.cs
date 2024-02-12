using CSDTP;
using CSDTP.Utils;

namespace ApiTypes.Communication.BaseTypes
{
    public class SerializableArray<T> : ISerializable<SerializableArray<T>> where T : ISerializable<T>,new()
    {
        public T[] Items { get; set; }

        public SerializableArray(params T[] items)
        {
            Items = items;
        }
        public SerializableArray()
        {

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
