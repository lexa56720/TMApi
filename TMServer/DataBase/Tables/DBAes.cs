using System;
using System.Collections.Generic;

namespace TMServer.DataBase.Tables;

public partial class DBAes
{
    public int CryptId { get; set; }

    public required byte[] AesKey { get; set; }

    public required byte[] IV { get; set; }

    public virtual DBUser User { get; set; } = null!;
}
