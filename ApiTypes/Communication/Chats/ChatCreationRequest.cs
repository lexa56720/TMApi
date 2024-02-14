using CSDTP;
using CSDTP.Utils;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Chats
{
    public class ChatCreationRequest : ISerializable<ChatCreationRequest>
    {
        public string ChatName { get; set; }
        public int[] Members { get; set; }

        public ChatCreationRequest()
        {

        }

        [SetsRequiredMembers]
        public ChatCreationRequest(string chatName, int[] members)
        {
            ChatName = chatName;
            Members = members;
        }

    }
}
