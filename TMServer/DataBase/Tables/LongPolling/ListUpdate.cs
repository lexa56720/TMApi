using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables.LongPolling
{
    public abstract class ListUpdate : Update
    {
        public required bool IsAdded { get; set; }

        public abstract int TargetId { get; }
    }
}
