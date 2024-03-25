using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables
{
    public partial class DBChatUser
    {
        public required int ChatId { get; set; }
        public required int UserId { get; set; }

        public virtual DBChat Chat { get; set; } = null!;
        public virtual DBUser User { get; set; } = null!;
    }
}
