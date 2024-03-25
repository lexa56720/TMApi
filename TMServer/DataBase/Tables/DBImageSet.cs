using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables
{
    public class DBImageSet
    {
        public int Id { get; set; }

        public virtual ICollection<DBImage> Images { get; set; } = new List<DBImage>();
    }
}