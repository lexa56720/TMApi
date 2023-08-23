using System;
using System.Collections.Generic;

namespace TMServer.DataBase.Tables;

public partial class RsaCrypt
{
    public required long Ip { get; set; }

    public required string PublicClientKey { get; set; }

    public required string PrivateServerKey { get; set; }

    public required DateTime CreateDate { get; set; }

}
