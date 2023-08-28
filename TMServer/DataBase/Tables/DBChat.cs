namespace TMServer.DataBase.Tables;

public partial class DBChat
{
    public int Id { get; set; }

    public int ChatId { get; set; }

    public required int MemberId { get; set; }

    public required int AdminId { get; set; }

    public string Name { get; set; }=string.Empty;

    public virtual DBUser Admin { get; set; } = null!;
    public virtual ICollection<DBUser> Members { get; set; } = new List<DBUser>();
    public virtual ICollection<DBMessage> Messages { get; set; } = new List<DBMessage>();
}
