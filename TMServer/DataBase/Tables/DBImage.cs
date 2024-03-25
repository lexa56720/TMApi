using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables
{
    public enum ImageSize
    {
        Small,
        Medium,
        Large,
    }

    public partial class DBImage
    {
        public int Id { get; set; }

        public int SetId { get; set; }

        public required string Url { get; set; } = null!;

        public required ImageSize Size { get; set; }

        public required byte[] Data { get; set; } = null!;

        public DBImageSet Set { get; set; } = null!;
    }
}
