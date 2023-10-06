using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables.LongPolling
{
    public partial class DBFriendRequestUpdate
    {
        public int Id { get; set; }
        public required int UserId { get; set; }

        public required int RequestId { get; set; }

        public virtual DBUser User { get; set; } = null!;

        public virtual DBFriendRequest FriendRequest { get; set; } = null!;
    }
}
