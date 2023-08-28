namespace TMServer.DataBase.Tables
{
    public class DBFriendRequest
    {
        public int Id { get; set; }
        public required int UserIdOne { get; set; }
        public required int UserIdTwo { get; set; }

        public virtual DBUser UserOne { get; set; } = null!;

        public virtual DBUser UserTwo { get; set; } = null!;
    }
}
