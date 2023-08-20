using CSDTP;
using CSDTP.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes
{
    internal class TMPacket<T> : Packet<T> where T : ISerializable<T>
    {
        public int Id { get; set; }


        protected override void DeserializeCustomData(BinaryReader reader)
        {
            base.DeserializeCustomData(reader);
            Id = reader.ReadInt32();
        }

        protected override void SerializeCustomData(BinaryWriter writer)
        {
            base.SerializeCustomData(writer);
            writer.Write(Id);
        }
    }
}
