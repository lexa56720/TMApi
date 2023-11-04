using CSDTP;

namespace ApiTypes.Communication.LongPolling
{
    public class LongPollingRequest : ISerializable<LongPollingRequest>
    {
        public int RandomInt { get; set; } = 148;
        public static LongPollingRequest Deserialize(BinaryReader reader)
        {
            return new LongPollingRequest()
            {
                RandomInt = reader.ReadInt32()
            };
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(RandomInt);
        }
    }
}
