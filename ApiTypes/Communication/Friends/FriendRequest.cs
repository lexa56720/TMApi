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

    }
}
