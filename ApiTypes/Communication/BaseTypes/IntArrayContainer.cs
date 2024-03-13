using CSDTP;
using CSDTP.Utils;

namespace ApiTypes.Communication.BaseTypes
{
    public class IntArrayContainer : ISerializable<IntArrayContainer>
    {
        public int[] Values { get; set; } = [];

        public IntArrayContainer(params int[] values)
        {
            Values = values;
        }
        public IntArrayContainer()
        {
        }

    }
}
