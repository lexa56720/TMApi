using System;
using System.Collections.Generic;

namespace TMServer.DataBase.Tables;

public partial class Chat
{
    public int Id { get; set; }

    public required int MemberId { get; set; }

    public virtual User Member { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
