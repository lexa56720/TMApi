using System;
using System.Collections.Generic;

namespace TMServer.DataBase.Tables;

public partial class Token
{
    public required int UserId { get; set; }

    public required string AccessToken { get; set; }

    public required DateOnly Expiration { get; set; }

    public virtual User User { get; set; } = null!;
}
