using System;
using System.Collections.Generic;

namespace TMServer.DataBase.Tables;

public partial class Token
{
    public int Id { get; set; }
    public required int UserId { get; set; }

    public required string AccessToken { get; set; }

    public required DateTime Expiration { get; set; }

    public virtual User User { get; set; } = null!;
}
