using ApiTypes.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables
{
    public class DBMessageMedia
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public MediaType MediaType { get; set; }
        public byte[] Data { get; set; } = null!;


        public virtual DBMessage Message { get; set; } = null!;
    }
}
