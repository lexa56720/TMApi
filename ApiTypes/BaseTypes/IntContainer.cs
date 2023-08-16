using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.BaseTypes
{
    public class IntContainer : ISerializable<IntContainer>
    {
        public int Value { get; init; }


        public IntContainer(int value)
        {
            Value = value;
        }
        public static IntContainer Deserialize(BinaryReader reader)
        {
            return new IntContainer(reader.ReadInt32());
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Value);
        }
    }
}
