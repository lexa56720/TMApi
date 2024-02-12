using CSDTP;

namespace ApiTypes.Communication.BaseTypes
{
    public class IntContainer : ISerializable<IntContainer>
    {
        public int Value { get; set; }


        public IntContainer(int value)
        {
            Value = value;
        }
        public IntContainer()
        {
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
