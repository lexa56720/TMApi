using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Messages
{
    public class MessagesRequest : ISerializable<MessagesRequest>
    {
        public required int[] Ids { get; set; }

        [SetsRequiredMembers]
        public MessagesRequest(int[] ids)
        {
            Ids = ids;
        }

        [SetsRequiredMembers]
        public MessagesRequest()
        {
        }
    }
}
