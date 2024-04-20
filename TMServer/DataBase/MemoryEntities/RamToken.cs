using TMServer.DataBase.Tables;

namespace TMServer.DataBase.MemoryEntities;

public class RamToken
{
    public int Id { get; set; }
    public required int UserId { get; set; }

    public required string AccessToken { get; set; }

    public required DateTime Expiration { get; set; }
}
