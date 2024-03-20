using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Chats
{
    public class ChatInvite : ISerializable<ChatInvite>
    {
        public int ChatId { get; set; }

        public int ToUserId { get; set; }

        public int FromUserId { get; set; }
        public int Id { get; set; }
        public ChatInvite()
        {

        }

        public ChatInvite(int chatId, int userId, int fromUserId,int id)
        {
            ChatId = chatId;
            ToUserId = userId;     
            FromUserId = fromUserId;
            Id = id;
        }
    }
}
