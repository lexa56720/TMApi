﻿namespace TMServer.DataBase.Tables;

public partial class DBMessage
{
    public int Id { get; set; }

    public required int AuthorId { get; set; }

    public required bool IsSystem { get; set; }
    public required string Content { get; set; }

    public required int DestinationId { get; set; }

    public required DateTime SendTime { get; set; }

    public virtual ICollection<DBMessageMedia> Medias { get; set; } = new List<DBMessageMedia>();

    public virtual DBUser Author { get; set; } = null!;

    public virtual DBChat Destination { get; set; } = null!;
}
