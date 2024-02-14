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
    }
}
