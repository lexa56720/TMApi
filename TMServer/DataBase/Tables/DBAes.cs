namespace TMServer.DataBase.Tables;

public partial class DBAes
{
    public int CryptId { get; set; }

    public required byte[] AesKey { get; set; }

    public required int UserId { get; set; }
    public bool IsDeprecated { get; set; }
    public DateTime DeprecatedDate { get; set; }

    public virtual DBUser User { get; set; } = null!;
}
