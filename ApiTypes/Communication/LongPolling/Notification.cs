using CSDTP;
using CSDTP.Utils;

namespace ApiTypes.Communication.LongPolling
{
    public class Notification : ISerializable<Notification>
    {
        public int LongPollId { get; set; }

        public int[] RelatedUserOnlineIds { get; set; } = [];
        public int[] RelatedUserOfflineIds { get; set; } = [];

        public int[] NewMessagesIds { get; set; } = [];
        public int[] ReadedMessagesIds { get; set; } = [];

        public int[] FriendRequestIds { get; set; } = [];
        public int[] NewFriendsIds { get; set; } = [];
        public int[] RemovedFriendsIds { get; set; } = [];

        public int[] ChatInviteIds { get; set; } = [];
        public int[] NewChatIds { get; set; } = [];
        public int[] RemovedChatIds { get; set; } = [];
        public int[] ChatChangedIds { get; set; } = [];

        public int[] RelatedUserChangedIds { get; set; } = [];

        public bool IsAny()
        {
            return
                RelatedUserOnlineIds.Length > 0
                || RelatedUserOfflineIds.Length > 0
                || NewMessagesIds.Length > 0
                || ReadedMessagesIds.Length > 0
                || FriendRequestIds.Length > 0
                || NewFriendsIds.Length > 0
                || RemovedFriendsIds.Length > 0
                || ChatInviteIds.Length > 0
                || NewChatIds.Length > 0
                || RemovedChatIds.Length > 0
                || ChatChangedIds.Length > 0
                || RelatedUserChangedIds.Length > 0;
        }

    }
}
