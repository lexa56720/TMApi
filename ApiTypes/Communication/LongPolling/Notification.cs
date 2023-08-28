using CSDTP;
using CSDTP.Utils;

namespace ApiTypes.Communication.LongPolling
{
    public class Notification : ISerializable<Notification>
    {
        public int[] MessagesIds { get; init; } = Array.Empty<int>();

        public int[] FriendRequestIds { get; init; } = Array.Empty<int>();

        public int[] ChatInviteIds { get; init; } = Array.Empty<int>();

        public int[] NewFriends { get; init; } = Array.Empty<int>();

        public int[] NewChats { get; init; } = Array.Empty<int>();

        public int[] FriendsProfileChanged { get; init; } = Array.Empty<int>();

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(MessagesIds);
            writer.Write(FriendRequestIds);
            writer.Write(ChatInviteIds);
            writer.Write(NewFriends);
            writer.Write(NewChats);
            writer.Write(FriendsProfileChanged);
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
                FriendsProfileChanged = reader.ReadInt32Array()
            };
        }
    }
}
