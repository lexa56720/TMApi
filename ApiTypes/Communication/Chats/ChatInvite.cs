using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Chats
{
    public class ChatInvite : ISerializable<ChatInvite>
    {
        public int ChatId { get; init; }

        public int ToUserId { get; init; }

        public int Id { get; set; } = -1;
        public ChatInvite()
        {

        }


        [SetsRequiredMembers]
        public ChatInvite(int chatId, int userId)
        {
            ChatId = chatId;
            ToUserId = userId;
        }

        [SetsRequiredMembers]
        public ChatInvite(int chatId, int userId,int id)
        {
            ChatId = chatId;
            ToUserId = userId;
            Id = id;
        }
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(ChatId);
            writer.Write(ToUserId);
            writer.Write(Id);
        }

        public static ChatInvite Deserialize(BinaryReader reader)
        {
            return new ChatInvite()
            {
                ChatId = reader.ReadInt32(),
                ToUserId = reader.ReadInt32(),
                Id=reader.ReadInt32(),
            };
        }
    }
}
