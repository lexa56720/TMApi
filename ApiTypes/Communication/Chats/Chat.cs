using ApiTypes.Communication.Users;
using CSDTP;
using CSDTP.Utils;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Chats
{
    public class Chat : ISerializable<Chat>
    {
        public int Id { get; set; }

        public int AdminId { get; set; }
        public int[] MemberIds { get; set; } = [];
        public int UnreadCount { get; set; }
        public bool IsDialogue { get; set; }
        public string Name { get; set; } = string.Empty;

        public Chat(int id, int adminId, bool isDialogue, string name, params int[] userIds)
        {
            Id = id;
            AdminId = adminId;
            MemberIds = userIds;
            IsDialogue = isDialogue;
            Name = name;
        }

        public Chat()
        {
        }
    }
}
