namespace TMServer.DataBase
{
    public class KeyPair
    {
        public required string ServerPrivateKey { get; init; }
        public required string ClientPublicKey { get; init; }
    }

}
