using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Messages
{
    public class MessageRequestById : ISerializable<MessageRequestById>
    {
        public required int[] Ids { get; set; }

        [SetsRequiredMembers]
        public MessageRequestById(int[] ids)
        {
            Ids = ids;
        }

        [SetsRequiredMembers]
        public MessageRequestById()
        {
        }
    }
}
