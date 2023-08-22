using System;
using System.Collections.Generic;

namespace TMServer.DataBase.Tables;

public partial class AesCrypt
{
    public int CryptId { get; set; }

    public required string AesKey { get; set; }

    public required string IV { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
