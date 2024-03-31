using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.ServerComponent.LongPolling
{
    public class LongPollResponseInfo
    {
        public int Id { get; init; }

        public int[] NewMessages { get; init; } = [];
        public int[] ReadedMessages { get; init; } = [];
        public int[] FriendRequests { get; init; } = [];
        public int[] FriendListUpdates { get; init; } = [];
        public int[] ChatListUpdates { get; init; } = [];
        public int[] RelatedUsersChanged { get; init; } = [];
        public int[] ChatsChanged { get; init; } = [];
        public int[] ChatInvites { get; init; } = [];
    }
}
