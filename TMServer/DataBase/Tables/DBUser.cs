namespace TMServer.DataBase.Tables;

public partial class DBUser
{
    public int Id { get; set; }

    public bool IsOnline => (DateTime.UtcNow - LastRequest).TotalSeconds < 180;
    public required string Name { get; set; }

    public required DateTime LastRequest { get; set; }

    public required DateTime RegisterDate { get; set; }

    public required string Login { get; set; }

    public required string Password { get; set; }

    public virtual ICollection<DBToken> Tokens { get; set; } = new List<DBToken>();

    public virtual ICollection<DBAes> Crypts { get; set; } = new List<DBAes>();

    public virtual ICollection<DBChat> Chats { get; set; } = new List<DBChat>();

    public DBFriend[] Friends => FriendsOne.Concat(FriendsTwo).ToArray();
    public virtual ICollection<DBFriend> FriendsOne { get; set; } = new List<DBFriend>();

    public virtual ICollection<DBFriend> FriendsTwo { get; set; } = new List<DBFriend>();

}
