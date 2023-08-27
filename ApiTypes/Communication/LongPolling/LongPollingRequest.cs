using CSDTP;

namespace ApiTypes.Communication.LongPolling
{
    public class LongPollingRequest : ISerializable<LongPollingRequest>
    {
        public static LongPollingRequest Deserialize(BinaryReader reader)
        {
            return new LongPollingRequest();
        }

        public void Serialize(BinaryWriter writer)
        {
            return;
        }
    }
}
