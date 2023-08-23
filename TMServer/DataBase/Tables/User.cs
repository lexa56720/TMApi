using System;
using System.Collections.Generic;

namespace TMServer.DataBase.Tables;

public partial class User
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required DateTime LastRequest { get; set; }

    public required int? CryptId { get; set; }

    public required string Login { get; set; }

    public required string Password { get; set; }

    public virtual Token? Token { get; set; }

    public virtual AesCrypt? Crypt { get; set; }

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<Friend> FriendsOne { get; set; } = new List<Friend>();

    public virtual ICollection<Friend> FriendsTwo { get; set; } = new List<Friend>();

}
