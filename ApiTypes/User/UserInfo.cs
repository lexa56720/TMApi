using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes
{
    public class UserInfo : ISerializable<UserInfo>
    {
        public User MainInfo { get; private set; }
        public User[] Friends { get; private set; } = Array.Empty<User>();

        public int FriendsCount { get; private set; }

        public int[] Chats { get; private set; } = Array.Empty<int>();

        public int ChatsCount { get; private set; }

        public UserInfo(User[] friends, int[] chats, User mainInfo)
        {
            Friends = friends;
            Chats = chats;

            FriendsCount = friends.Length;
            ChatsCount = chats.Length;
            MainInfo = mainInfo;
        }
        public UserInfo()
        {

        }
        public static UserInfo Deserialize(BinaryReader reader)
        {
            var userInfo = new UserInfo()
            {
                FriendsCount = reader.ReadInt32(),
                ChatsCount = reader.ReadInt32(),
                MainInfo=User.Deserialize(reader)
            };

            userInfo.Friends = new User[userInfo.FriendsCount];
            for (int i = 0; i < userInfo.FriendsCount; i++)
                userInfo.Friends[i] = User.Deserialize(reader);

            userInfo.Chats= new int[userInfo.ChatsCount];
            for (int i = 0; i < userInfo.ChatsCount; i++)
                userInfo.Chats[i] = reader.ReadInt32();
            return userInfo;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(FriendsCount);
            writer.Write(ChatsCount);
            MainInfo.Serialize(writer);

            for (int i = 0; i < FriendsCount; i++)
                Friends[i].Serialize(writer);

            for (int i = 0; i < ChatsCount; i++)
                writer.Write(Chats[i]);
        }
    }
}
