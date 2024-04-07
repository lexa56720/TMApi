using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables.FileTables
{
    public enum ImageSize
    {
        Small,
        Medium,
        Large,
    }

    public partial class DBImage : File
    {
        public int? SetId { get; set; }

        public required ImageSize Size { get; set; }

        public virtual DBImageSet? Set { get; set; } = null!;
    }
}
