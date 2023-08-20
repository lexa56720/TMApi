﻿using CSDTP;
using CSDTP.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Users
{
    public class UserInfo : ISerializable<UserInfo>
    {
        public required User MainInfo { get; init; }
        public required User[] Friends { get; init; } = Array.Empty<User>();

        public required int[] Chats { get; init; } = Array.Empty<int>();

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
            writer.Write(Chats);
        }
        public static UserInfo Deserialize(BinaryReader reader)
        {
            return new UserInfo()
            {
                MainInfo=User.Deserialize(reader),
                Friends=reader.Read<User>(),
                Chats=reader.ReadInt32Array(),
            };
        }


    }
}
