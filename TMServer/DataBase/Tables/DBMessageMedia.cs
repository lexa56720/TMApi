using ApiTypes.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables
{
    public partial class DBMessageMedia
    {
        public int Id { get; set; }
        public int MediaId { get; set; }

        public int MessageId { get; set; }

        public virtual DBMessage Message { get; set; } = null!;
    }
}
