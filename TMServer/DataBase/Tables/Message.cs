using System;
using System.Collections.Generic;

namespace TMServer.DataBase.Tables;

public partial class Message
{
    public int Id { get; set; }

    public required int AuthorId { get; set; }

    public required string Content { get; set; }

    public required int DestinationId { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual Chat Destination { get; set; } = null!;
}
