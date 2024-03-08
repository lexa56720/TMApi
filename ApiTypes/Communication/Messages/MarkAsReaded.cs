using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Messages
{
    public class MarkAsReaded : ISerializable<MarkAsReaded>
    {
        public int[] MessageIds { get; set; }

        public MarkAsReaded()
        {
        }
        public MarkAsReaded(int[] messageIds)
        {
            MessageIds = messageIds;
        }
    }
}
