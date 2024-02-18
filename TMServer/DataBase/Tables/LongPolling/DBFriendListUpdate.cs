using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables.LongPolling
{
    public class DBFriendListUpdate : DBUpdate
    {
        public int Id { get; set; }

        public required bool IsAdded { get; set; }

        public required int FriendId { get; set; }

        public virtual DBUser Friend{ get; set; } = null!;
    }
}
