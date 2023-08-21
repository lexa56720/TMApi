using CSDTP;
using CSDTP.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes
{
    public class TMPacket<T> : Packet<T> where T : ISerializable<T>
    {
        public static IdHolder Id { get; set; } = new IdHolder(-1);

        public class IdHolder
        {
            public IdHolder(int id)
            {
                Value = id;
            }
            public static int Value { get; set; }
        }

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
