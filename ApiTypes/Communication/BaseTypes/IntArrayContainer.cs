using CSDTP;
using CSDTP.Utils;

namespace ApiTypes.Communication.BaseTypes
{
    public class IntArrayContainer : ISerializable<IntArrayContainer>
    {

        public int[] Values { get; private set; }

        public int Count { get; private set; }


        public IntArrayContainer(params int[] values)
        {
            Values = values;
            Count = values.Length;
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
