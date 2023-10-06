namespace TMServer.DataBase.Tables;

public partial class DBRsa
{
    public int Id { get; set; }

    public required string PublicClientKey { get; set; }

    public required string PrivateServerKey { get; set; }

    public required DateTime Expiration { get; set; }

}
