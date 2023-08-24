using System;
using System.Collections.Generic;

namespace TMServer.DataBase.Tables;

public partial class DBUser
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required DateTime LastRequest { get; set; }

    public required int? CryptId { get; set; }

    public required string Login { get; set; }

    public required string Password { get; set; }

    public virtual DBToken? Token { get; set; }

    public virtual DBAes? Crypt { get; set; }

    public virtual ICollection<DBChat> Chats { get; set; } = new List<DBChat>();

    public virtual ICollection<DBFriend> FriendsOne { get; set; } = new List<DBFriend>();

    public virtual ICollection<DBFriend> FriendsTwo { get; set; } = new List<DBFriend>();

}
