using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Messages
{
    public class MessageRequestByChats: ISerializable<MessageRequestByChats>
    {
        public int[] Ids { get; set; } = [];

        public MessageRequestByChats(int[] ids)
        {
            Ids = ids;
        }
        public MessageRequestByChats()
        {
        }
    }
}
