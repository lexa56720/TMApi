using ApiTypes.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables
{
    public class DBMessageAction
    {
        public int Id { get; set; }

        public required ActionKind Kind { get; set; }
        public int MessageId { get; set; }
        public required int ExecutorId { get; set; }
        public int TargetId { get; set; }

        public virtual DBUser Executor { get; set; } = null!;

        public virtual DBUser? Target { get; set; } = null!;

        public required virtual DBMessage Message { get; set; } = null!;
    }
}
