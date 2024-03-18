 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables.LongPolling
{
    public class DBNewMessageUpdate:Update
    {
        public required int MessageId { get; set; }
        public virtual DBMessage Message { get; set; } = null!;
    }
}
