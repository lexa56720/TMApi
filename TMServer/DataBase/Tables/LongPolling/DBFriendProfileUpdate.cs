﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables.LongPolling
{
    public partial class DBFriendProfileUpdate
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public required int FriendId { get; set; }

        public virtual DBUser User { get; set; } = null!;
        public virtual DBUser Friend { get; set; } = null!;
    }
}
