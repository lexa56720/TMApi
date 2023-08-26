using CSDTP;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes
{
    public enum RequestHeaders
    {
        None,

        LongPoll,

        GetUserInfo,
        GetUser,
        GetUserMany,

        GetLastMessages,
        SendMessage,

        GetFriendRequest,
        GetFriendRequestMany,
        ResponseFriendRequest,
        SendFriendRequest,

        CreateChat,
        GetChat,
        GetChatMany,
        SendChatInvite,
        GetChatInvite,
        GetChatInviteMany,
    }
    public class ApiData<T> : ISerializable<ApiData<T>> where T : ISerializable<T>
    {
        public RequestHeaders Header { get; init; } = RequestHeaders.None;
        public required string Token { get; init; }

        public required int UserId { get; init; }

        public required T Data { get; init; }

        [SetsRequiredMembers]
        public ApiData(string token, int id, T data)
        {
            Token = token;
            Data = data;
            UserId = id;
        }
        [SetsRequiredMembers]
        public ApiData(RequestHeaders header, string token, int id, T data)
        {
            Header = header;
            Token = token;
            Data = data;
            UserId = id;
        }

        public ApiData()
        {

        }

        public static ApiData<T> Deserialize(BinaryReader reader)
        {
            return new ApiData<T>()
            {
                Header = (RequestHeaders)reader.ReadByte(),
                Token = reader.ReadString(),
                UserId = reader.Read(),
                Data = T.Deserialize(reader)
            };
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write((byte)Header);
            writer.Write(Token);
            writer.Write(UserId);
            Data.Serialize(writer);
        }
    }
}
