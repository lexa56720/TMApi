﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables.LongPolling
{
    public class DBChatUpdate : DBUpdate
    {
        public int Id { get; set; }
        public required bool IsAdded { get; set; }
        public required int ChatId { get; set; }
        public virtual DBChat Chat { get; set; } = null!;
    }
}
