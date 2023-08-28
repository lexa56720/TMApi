using ApiTypes.Communication.Users;
using CSDTP;
using CSDTP.Utils;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Chats
{
    public class Chat : ISerializable<Chat>
    {
        public required int[] MemberIds { get; init; } = Array.Empty<int>();

        public required int AdminId { get; init; }

        public required int Id { get; init; }

        public required int TotalMessages { get; init; }

        [SetsRequiredMembers]
        public Chat(int id, int adminId, params int[] userIds)
        {
            Id = id;
            AdminId = adminId;
            MemberIds = userIds;
        }

        public Chat()
        {
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(TotalMessages);
            writer.Write(AdminId);
            writer.Write(MemberIds);    
        }

        public static Chat Deserialize(BinaryReader reader)
        {
            var chat = new Chat()
            {
                Id = reader.ReadInt32(),
                TotalMessages = reader.ReadInt32(),
                AdminId = reader.ReadInt32(),
                MemberIds = reader.ReadInt32Array()
            };
            return chat;
        }
    }
}
