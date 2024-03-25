using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables.LongPolling
{
    public partial class DBChatUpdate : Update
    {
        public required int ChatId { get; set; }
        public virtual DBChat Chat { get; set; } = null!;
    }
}
