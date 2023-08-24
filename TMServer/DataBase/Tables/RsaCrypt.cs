using System;
using System.Collections.Generic;

namespace TMServer.DataBase.Tables;

public partial class RsaCrypt
{
    public int Id { get; set; }

    public required string PublicClientKey { get; set; }

    public required string PrivateServerKey { get; set; }

    public required DateTime CreateDate { get; set; }

}
