using CSDTP;
using CSDTP.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.LongPolling
{
    public class Notification : ISerializable<Notification>
    {
        public int[] MessagesIds { get; init; } = Array.Empty<int>();

        public int[] FriendRequestIds { get; init; } = Array.Empty<int>();

        public int[] ChatInviteIds { get; init; } = Array.Empty<int>();

        public int[] NewFriends { get; init; } = Array.Empty<int>();

        public int[] NewChats { get; init; } = Array.Empty<int>();

        public int[] FriendsStatusChanged { get; init; } = Array.Empty<int>();

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(MessagesIds);
            writer.Write(FriendRequestIds);
            writer.Write(ChatInviteIds);
            writer.Write(NewFriends);
            writer.Write(NewChats);
            writer.Write(FriendsStatusChanged);
        }

        public static Notification Deserialize(BinaryReader reader)
        {
            return new Notification()
            {
                MessagesIds = reader.ReadInt32Array(),
                FriendRequestIds = reader.ReadInt32Array(),
                ChatInviteIds = reader.ReadInt32Array(),
                NewFriends = reader.ReadInt32Array(),
                NewChats = reader.ReadInt32Array(),
                FriendsStatusChanged = reader.ReadInt32Array()
            };
        }
    }
}
