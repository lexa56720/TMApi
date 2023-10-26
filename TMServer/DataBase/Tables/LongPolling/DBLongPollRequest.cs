using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables.LongPolling
{
    public partial class DBLongPollRequest
    {
        public int Id { get; set; }

        public required DateTime CreateDate { get; set; }
        public required int UserId { get; set; }

        public required string DataType { get; set; }

        public required byte[] RequestPacket { get; set; }

        public virtual DBUser User { get; set; } = null!;
    }
}
