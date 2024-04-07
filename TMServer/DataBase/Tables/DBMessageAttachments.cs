using ApiTypes.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables
{
    public enum AttachmentKind
    {
        Image,
        File,
    }
    public partial class DBMessageAttachments
    {
        public int Id { get; set; }
        public int AttachmentId { get; set; }
        public AttachmentKind Kind { get; set; }
        public int MessageId { get; set; }
        public virtual DBMessage Message { get; set; } = null!;
    }
}
