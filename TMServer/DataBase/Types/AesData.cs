namespace TMServer.DataBase.Types
{
    internal class AesData
    {
        public required byte[] Key { get; init; }

        public required byte[] IV { get; init; }
    }
}
