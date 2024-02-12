using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Chats
{
    public class ChatInvite : ISerializable<ChatInvite>
    {
        public int ChatId { get; set; }

        public int ToUserId { get; set; }

        public int FromUserId { get; set; }
        public int Id { get; set; } = -1;
        public ChatInvite()
        {

        }


        [SetsRequiredMembers]
        public ChatInvite(int chatId, int userId, int fromUserId)
        {
            ChatId = chatId;
            ToUserId = userId;
            FromUserId = fromUserId;
        }

        [SetsRequiredMembers]
        public ChatInvite(int chatId, int userId, int fromUserId,int id)
        {
            ChatId = chatId;
            ToUserId = userId;     
            FromUserId = fromUserId;
            Id = id;
        }
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(ChatId);
            writer.Write(ToUserId);
            writer.Write(FromUserId);
            writer.Write(Id);
        }

        public static ChatInvite Deserialize(BinaryReader reader)
        {
            return new ChatInvite()
            {
                ChatId = reader.ReadInt32(),
                ToUserId = reader.ReadInt32(),
                FromUserId=reader.ReadInt32(),
                Id =reader.ReadInt32(),
            };
        }
    }
}
