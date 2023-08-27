using TMServer.DataBase;

namespace TMServer
{
    internal class Program
    {

        private static void Main(string[] args)
        {
            var db = new TmdbContext();
            ////db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            Servers.TMServer server = new Servers.TMServer(GlobalSettings.AuthPort, GlobalSettings.ApiPort);
            server.Start();
            Console.ReadLine();
        }
    }
}