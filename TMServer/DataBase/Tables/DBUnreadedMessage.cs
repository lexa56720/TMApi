using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Tables.LongPolling;

namespace TMServer.DataBase.Tables
{
    public partial class DBUnreadedMessage
    {
        public int Id { get; set; }

        public required int UserId { get; set; }
        public int MessageId { get; set; }
        public virtual DBMessage Message { get; set; } = null!;
        public virtual DBUser User { get; set; } = null!;

    }
}
