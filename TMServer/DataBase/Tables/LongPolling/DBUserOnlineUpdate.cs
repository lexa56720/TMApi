using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables.LongPolling
{
    public class DBUserOnlineUpdate : ListUpdate
    {
        public required int RelatedUserId { get; set; }
        public virtual DBUser RelatedUser { get; set; } = null!;
        public override int TargetId => RelatedUserId;
    }
}
