using CSDTP;
using CSDTP.Utils;

namespace ApiTypes.Communication.LongPolling
{
    public class Notification : ISerializable<Notification>
    {
        public int[] MessagesIds { get; set; } = [];

        public int[] FriendRequestIds { get; set; } = [];

        public int[] ChatInviteIds { get; set; } = [];

        public int[] NewFriends { get; set; } = [];

        public int[] RemovedFriends { get; set; } = [];

        public int[] NewChats { get; set; } = [];

        public int[] FriendsProfileChanged { get; set; } = [];

    }
}
