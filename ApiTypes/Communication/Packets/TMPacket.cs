using CSDTP;
using CSDTP.Packets;

namespace ApiTypes.Communication.Packets
{
    public class TMPacket<T> : Packet<T>, ITMPacket where T : ISerializable<T>,new()
    {
        public int Id { get; set; } =-1;

        public override void DeserializeUnprotectedCustomData(BinaryReader reader)
        {
            base.DeserializeUnprotectedCustomData(reader);
            Id = reader.ReadInt32();
        }

        public override void SerializeUnprotectedCustomData(BinaryWriter writer)
        {
            base.SerializeUnprotectedCustomData(writer);
            writer.Write(Id);
        }
    }
}
