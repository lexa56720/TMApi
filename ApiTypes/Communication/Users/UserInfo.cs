using CSDTP;
using CSDTP.Utils;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Users
{
    public class UserInfo : ISerializable<UserInfo>
    {
        public  User MainInfo { get; set; }

        public  User[] Friends { get; set; } = [];
        public  int[] FriendRequests { get; set; } = [];

        public  int[] Chats { get; set; } = [];

        public  int[] ChatInvites { get;set; } = [];


        [SetsRequiredMembers]
        public UserInfo(User[] friends, int[] chats, User mainInfo)
        {
            Friends = friends;
            Chats = chats;
            MainInfo = mainInfo;
        }
        public UserInfo()
        {

        }

    }
}
