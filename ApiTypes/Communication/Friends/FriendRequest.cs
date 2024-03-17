using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Friends
{
    public class FriendRequest : ISerializable<FriendRequest>
    {
        public int Id { get; set; } = -1;

        public int FromId { get; set; } = -1;
        public int ToId { get; set; }

        public FriendRequest(int toId)
        {
            ToId = toId;
        }
        public FriendRequest(int toId,int fromId, int id)
        {
            ToId = toId;
            FromId = fromId;
            Id = id;
        }
        public FriendRequest()
        {

        }

    }
}
