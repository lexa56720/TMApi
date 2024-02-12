using CSDTP;
using CSDTP.Utils;

namespace ApiTypes.Communication.BaseTypes
{
    public class IntArrayContainer : ISerializable<IntArrayContainer>
    {

        public int[] Values { get; set; }

        public int Count { get; set; }


        public IntArrayContainer(params int[] values)
        {
            Values = values;
            Count = values.Length;
        }
        public IntArrayContainer()
        {
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Values);
        }

        public static IntArrayContainer Deserialize(BinaryReader reader)
        {
            return new IntArrayContainer(reader.ReadInt32Array());
        }
    }
}
