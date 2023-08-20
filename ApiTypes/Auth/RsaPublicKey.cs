using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Auth
{
    public class RsaPublicKey : ISerializable<RsaPublicKey>
    {
        public string Key { get; private set; }


        public RsaPublicKey(string key)
        {
            Key = key;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Key);
        }

        public static RsaPublicKey Deserialize(BinaryReader reader)
        {
            return new RsaPublicKey(reader.ReadString());
        }
    }
}
