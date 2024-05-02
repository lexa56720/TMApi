using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.BaseTypes
{
    public class SerializableFile : ISerializable<SerializableFile>
    {
        public string Name { get; set; } = string.Empty;
        public byte[] Data { get; set; } = [];

        public SerializableFile(string name, byte[] data)
        {
            Name= name;
            Data= data; 
        }

        public SerializableFile() { }
    }
}
