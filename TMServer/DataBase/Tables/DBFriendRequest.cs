namespace TMServer.DataBase.Tables
{
    public class DBFriendRequest
    {
        public int Id { get; set; }
        public required int SenderId { get; set; }
        public required int ReceiverId { get; set; }

        public virtual DBUser Sender { get; set; } = null!;

        public virtual DBUser Receiver { get; set; } = null!;
    }
}
