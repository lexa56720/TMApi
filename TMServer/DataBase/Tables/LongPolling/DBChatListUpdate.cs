using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables.LongPolling
{
    public class DBChatListUpdate : ListUpdate
    {
        public required int ChatId { get; set; }
        public virtual DBChat Chat { get; set; } = null!;

        public override int TargetId => ChatId;
    }
}
