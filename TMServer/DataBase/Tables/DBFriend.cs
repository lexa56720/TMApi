namespace TMServer.DataBase.Tables;

public partial class DBFriend
{
    public int Id { get; set; }
    public required int SenderId { get; set; }
    public required int DestId { get; set; }

    public virtual DBUser Sender { get; set; } = null!;

    public virtual DBUser Dest { get; set; } = null!;
}
