﻿using System;
using System.Collections.Generic;

namespace TMServer.DataBase.Tables;

public partial class DBFriend
{
    public int Id { get; set; }
    public required int UserIdOne { get; set; }
    public required int UserIdTwo { get; set; }

    public DBUser UserOne { get; set; } = null!;

    public DBUser UserTwo { get; set; } = null!;
}