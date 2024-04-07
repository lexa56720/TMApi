using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables.FileTables
{
    public partial class DBBinaryFile:File
    {
        public string Name { get; set; } = string.Empty;
    }
}
