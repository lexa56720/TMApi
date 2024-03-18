using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables.LongPolling
{
    public partial class DBFriendRequestUpdate: DBUpdate
    {
        public required int RequestId { get; set; }

        public virtual DBFriendRequest FriendRequest { get; set; } = null!;
    }
}
