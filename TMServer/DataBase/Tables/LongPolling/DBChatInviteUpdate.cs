using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables.LongPolling
{
    public partial class DBChatInviteUpdate : Update
    {
        public required int ChatInviteId { get; set; }

        public virtual DBChatInvite Invite { get; set; } = null!;
    }
}
