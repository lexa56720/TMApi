using CSDTP;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CSDTP.Utils;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Chats
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

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(ChatName);
            writer.Write(Members);
        }


        public static ChatCreationRequest Deserialize(BinaryReader reader)
        {
            return new ChatCreationRequest()
            {
                ChatName = reader.ReadString(),
                Members = reader.ReadInt32Array(),
            };

        }

    }
}
