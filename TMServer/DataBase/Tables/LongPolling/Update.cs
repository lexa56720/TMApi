﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables.LongPolling
{
    public abstract class Update
    {
        public int Id { get; set; }

        public required int UserId { get; set; }

        public virtual DBUser User { get; set; } = null!;
    }
}
