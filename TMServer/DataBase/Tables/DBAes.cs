﻿namespace TMServer.DataBase.Tables;

public partial class DBAes
{
    public int Id { get; set; }

    public required byte[] AesKey { get; set; }

    public required int UserId { get; set; }
    public required DateTime Expiration { get; set; }
    public virtual DBUser User { get; set; } = null!;
}
