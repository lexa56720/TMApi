using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables.FileTables
{
    public abstract class File
    {
        public int Id { get; set; }

        public required string Url { get; set; } = null!;

        public required byte[] Data { get; set; } = null!;
    }
}
