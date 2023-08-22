using System;
using System.Collections.Generic;

namespace TMServer.DataBase.Tables;

public partial class User
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required bool IsOnline { get; set; }

    public required int CryptId { get; set; }

    public required string Login { get; set; }

    public required string Password { get; set; }

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual AesCrypt Crypt { get; set; } = null!;

    public virtual ICollection<Friend> FriendsNavigations { get; set; } = new List<Friend>();

    public virtual Friend? FriendUser { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual Token? Token { get; set; }
}
