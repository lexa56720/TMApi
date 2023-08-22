using System;
using System.Collections.Generic;

namespace TMServer.DataBase.Tables;

public partial class Friend
{
    public required int Userid { get; set; }

    public required int FriendId { get; set; }

    public virtual User FriendNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
