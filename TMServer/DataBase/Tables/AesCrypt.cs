using System;
using System.Collections.Generic;

namespace TMServer.DataBase.Tables;

public partial class AesCrypt
{
    public int CryptId { get; set; }

    public required string AesKey { get; set; }

    public required string IV { get; set; }

    public virtual User User { get; set; } = null!;
}
