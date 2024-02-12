﻿using CSDTP;
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

        public void Serialize(BinaryWriter writer)
        {
            MainInfo.Serialize(writer);

            writer.Write(Friends);
            writer.Write(FriendRequests);
            writer.Write(Chats);
            writer.Write(ChatInvites);
        }
        public static UserInfo Deserialize(BinaryReader reader)
        {
            return new UserInfo()
            {
                MainInfo = User.Deserialize(reader),
                Friends = reader.Read<User>(),
                FriendRequests=reader.ReadInt32Array(),
                Chats = reader.ReadInt32Array(),
                ChatInvites = reader.ReadInt32Array(),
            };
        }

    }
}
