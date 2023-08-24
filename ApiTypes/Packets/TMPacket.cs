using CSDTP;
using CSDTP.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Packets
{
    public class TMPacket<T> : Packet<T>, ITMPacket where T : ISerializable<T>
    {
        public IdHolder Id { get; private set; } = new IdHolder(-1);

        protected override void DeserializeCustomData(BinaryReader reader)
        {
            base.DeserializeCustomData(reader);
            Id = new IdHolder(reader.ReadInt32());
        }

        protected override void SerializeCustomData(BinaryWriter writer)
        {
            base.SerializeCustomData(writer);
            writer.Write(IdHolder.Value);
        }
    }
}
