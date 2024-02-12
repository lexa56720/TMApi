using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Friends
{
    public class FriendRequest : ISerializable<FriendRequest>
    {
        public int Id { get; set; } = -1;
        public  int FromId { get; set; }
        public  int ToId { get; set; }

        [SetsRequiredMembers]
        public FriendRequest(int fromId, int toId)
        {
            FromId = fromId;
            ToId = toId;
        }
        [SetsRequiredMembers]
        public FriendRequest(int fromId, int toId,int id)
        {
            FromId = fromId;
            ToId = toId;
            Id = id;
        }
        public FriendRequest()
        {

        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(FromId);
            writer.Write(ToId);
            writer.Write(Id);
        }

        public static FriendRequest Deserialize(BinaryReader reader)
        {
            return new FriendRequest()
            {
                FromId = reader.ReadInt32(),
                ToId = reader.ReadInt32(),
                Id= reader.ReadInt32(),
            };
        }
    }
}
