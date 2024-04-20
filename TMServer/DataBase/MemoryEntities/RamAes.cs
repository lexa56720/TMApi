using TMServer.DataBase.Tables;

namespace TMServer.DataBase.MemoryEntities;

public class RamAes
{
    public int Id { get; set; }

    public required byte[] AesKey { get; set; }

    public required int UserId { get; set; }
    public required DateTime Expiration { get; set; }
}
