using CSDTP;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Friends
{
    public class FriendRequest : ISerializable<FriendRequest>
    {
        public required int FromId { get; init; }
        public required int ToId { get; init; }

        [SetsRequiredMembers]
        public FriendRequest(int fromId, int toId)
        {
            FromId = fromId;
            ToId = toId;
        }

        public FriendRequest()
        {

        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(FromId);
            writer.Write(ToId);
        }

        public static FriendRequest Deserialize(BinaryReader reader)
        {
            return new FriendRequest()
            {
                FromId = reader.ReadInt32(),
                ToId = reader.ReadInt32()
            };
        }
    }
}
