using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables.LongPolling
{
    public partial class DBUserProfileUpdate : Update
    {
        public required int ProfileId { get; set; }

        public virtual DBUser Profile { get; set; } = null!;
    }
}
