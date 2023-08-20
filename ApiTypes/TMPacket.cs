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
        public static int Id { get; set; } = -1;


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
