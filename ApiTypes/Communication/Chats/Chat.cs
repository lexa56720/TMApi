using ApiTypes.Communication.Users;
using CSDTP;
using CSDTP.Utils;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Chats
{
    public class Chat : ISerializable<Chat>
    {
        public required int Id { get; init; }

        public required int AdminId { get; init; }
        public required int[] MemberIds { get; init; } = Array.Empty<int>();

        public required bool IsDialogue { get; init; }
        public string Name { get; set; } = string.Empty;


        [SetsRequiredMembers]
        public Chat(int id, int adminId, bool isDialogue, string name, params int[] userIds)
        {
            Id = id;
            AdminId = adminId;
            MemberIds = userIds;
            IsDialogue=isDialogue;
            Name = name;
        }

        public Chat()
        {
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(AdminId);
            writer.Write(MemberIds);
            writer.Write(IsDialogue);
            writer.Write(Name);
        }

        public static Chat Deserialize(BinaryReader reader)
        {
            var chat = new Chat()
            {
                Id = reader.ReadInt32(),
                AdminId = reader.ReadInt32(),
                MemberIds = reader.ReadInt32Array(),
                IsDialogue = reader.ReadBoolean(),
                Name = reader.ReadString(),
            };
            return chat;
        }
    }
}
