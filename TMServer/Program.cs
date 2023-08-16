namespace TMServer
{
    internal class Program
    {
        static int AuthPort = 6665;
        static int ResponsePort = 6666;
        static void Main(string[] args)
        {
            Servers.TMServer server = new Servers.TMServer(AuthPort, ResponsePort);
            server.Start();
        }
    }
}