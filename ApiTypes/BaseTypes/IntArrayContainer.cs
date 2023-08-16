using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.BaseTypes
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
            writer.Write(Values.Length);
            for (int i = 0; i < Values.Length; i++)
                writer.Write(Values[i]);
        }

        public static IntArrayContainer Deserialize(BinaryReader reader)
        {
            var length = reader.ReadInt32();
            var values = new int[length];

            for (int i = 0; i < length; i++)
                values[i] = reader.ReadInt32();
            return new IntArrayContainer(values);
        }
    }
}
