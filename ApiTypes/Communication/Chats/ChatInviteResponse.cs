using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Chats
{
    public class ChatInviteResponse : ISerializable<ChatInviteResponse>
    {
        public bool IsAccepted { get; init; }

        public ChatInvite Invite { get; init; }

        public ChatInviteResponse()
        {
        }

        [SetsRequiredMembers]
        public ChatInviteResponse(ChatInvite invite, bool isAccepted)
        {
            Invite = invite;
            IsAccepted = isAccepted;
        }

        public static ChatInviteResponse Deserialize(BinaryReader reader)
        {
            return new ChatInviteResponse()
            {
                IsAccepted = reader.ReadBoolean(),
                Invite = ChatInvite.Deserialize(reader)
            };
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(IsAccepted);
            Invite.Serialize(writer);
        }
    }
}
