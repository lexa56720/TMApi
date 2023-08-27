using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Auth
{
    public class RsaPublicKey : ISerializable<RsaPublicKey>
    {
        public required string Key { get; init; }

        public int Id { get; init; } = -1;

        [SetsRequiredMembers]
        public RsaPublicKey(string key)
        {
            Key = key;
        }
        [SetsRequiredMembers]
        public RsaPublicKey(string key, int id)
        {
            Key = key;
            Id = id;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Key);
            writer.Write(Id);
        }

        public static RsaPublicKey Deserialize(BinaryReader reader)
        {
            return new RsaPublicKey(reader.ReadString(), reader.ReadInt32());
        }
    }
}
