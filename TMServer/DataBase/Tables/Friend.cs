using System;
using System.Collections.Generic;

namespace TMServer.DataBase.Tables;

public partial class Friend
{
    public int Id { get; set; }
    public required int UserIdOne { get; set; }
    public required int UserIdTwo { get; set; }

    public User UserOne { get; set; } = null!;

    public User UserTwo { get; set; } = null!;
}
