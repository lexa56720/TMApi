using CSDTP;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Auth
{
    public class RsaPublicKey : ISerializable<RsaPublicKey>
    {
        public required string Key { get; init; }


        [SetsRequiredMembers]
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
