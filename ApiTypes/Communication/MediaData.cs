using CSDTP;
using CSDTP.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication
{
    public enum MediaType
    {
        Image,
        Video,
        Audio,
        File,
    }
    public class MediaData : ISerializable<MediaData>
    {
        public required byte[] Data { get; init; }

        public required MediaType Type { get; init; }

        public MediaData()
        {

        }

        [SetsRequiredMembers]
        public MediaData(byte[] data,MediaType type)
        {
            Data=data; 
            Type = type;
        }


        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Data);
            writer.Write((byte)Type);
        }
        public static MediaData Deserialize(BinaryReader reader)
        {
            return new MediaData()
            {
                Data = reader.ReadByteArray(),
                Type = (MediaType)reader.ReadByte(),
            };

        }
    }
}
