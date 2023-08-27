using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Chats
{
    public class ChatInvite : ISerializable<ChatInvite>
    {
        public int ChatId { get; init; }

        public int ToUserId { get; init; }


        public ChatInvite()
        {

        }


        [SetsRequiredMembers]
        public ChatInvite(int chatId, int userId)
        {
            ChatId = chatId;
            ToUserId = userId;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(ChatId);
            writer.Write(ToUserId);
        }

        public static ChatInvite Deserialize(BinaryReader reader)
        {
            return new ChatInvite()
            {
                ChatId = reader.ReadInt32(),
                ToUserId = reader.ReadInt32(),
            };
        }
    }
}
