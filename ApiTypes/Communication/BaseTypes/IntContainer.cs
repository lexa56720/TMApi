using CSDTP;

namespace ApiTypes.Communication.BaseTypes
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
