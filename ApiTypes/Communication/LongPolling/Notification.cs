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

        public int[] NewChats { get; set; } = [];

        public int[] FriendsProfileChanged { get; set; } = [];

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
