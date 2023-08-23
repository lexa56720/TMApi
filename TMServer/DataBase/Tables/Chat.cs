using System;
using System.Collections.Generic;

namespace TMServer.DataBase.Tables;

public partial class Chat
{
    public int Id { get; set; }

    public int ChatId { get; set; }

    public required int MemberId { get; set; }

    public required int AdminId { get; set; }

    public virtual User Admin { get; set; } = null!;
    public virtual ICollection<User> Members { get; set; } = new List<User>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
