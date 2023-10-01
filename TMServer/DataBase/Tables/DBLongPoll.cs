using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables
{
    internal class DBLongPoll
    {
        public DateTime RequestTime { get; set; }

        public int UserId { get; set; }

        public string DataType { get; set; }

        public byte[] RequestPacket { get; set; }

        public virtual DBUser User { get; set; }
    }
}
