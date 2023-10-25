﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables.LongPolling
{
    public partial class DBChatUpdate
    {
        public int Id { get; set; }
        public required int UserId { get; set; }

        public required int MessageId {  get; set; }

        public virtual DBMessage Message { get; set; } = null!;
        public virtual DBUser User { get; set; } = null!;
    }
}