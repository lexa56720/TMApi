using ApiTypes.Communication.Users;
using TMServer.DataBase.MemoryEntities;

namespace TMServer.DataBase.Tables;

public partial class DBUser
{
    public int Id { get; set; }

    public bool IsOnline =>  (DateTime.UtcNow - LastRequest).Duration()<Settings.OnlineTimeout;

    public int ProfileImageId { get; set; }

    public required string Name { get; set; }

    public required DateTime LastRequest { get; set; }

    public required DateTime RegisterDate { get; set; }

    public required string Login { get; set; }

    public required string Password { get; set; }

    public virtual ICollection<RamToken> Tokens { get; set; } = new List<RamToken>();

    public virtual ICollection<RamAes> Crypts { get; set; } = new List<RamAes>();

    public virtual ICollection<DBChat> Chats { get; set; } = new List<DBChat>();

    public virtual ICollection<DBFriend> FriendsOne { get; set; } = new List<DBFriend>();

    public virtual ICollection<DBFriend> FriendsTwo { get; set; } = new List<DBFriend>();

    public IEnumerable<DBUser> GetFriends()
    {
        var friends = FriendsOne.Concat(FriendsTwo)
                                .Select(f => f.SenderId == Id ? f.Receiver : f.Sender);
        return friends;
    }

}
