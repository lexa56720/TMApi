using ApiTypes.Communication.BaseTypes;
using TMServer.DataBase;
using TMServer.Logger;

namespace TMServer
{
    internal class Program
    {

        private static void Main(string[] args)
        {
            ILogger log = new ConsoleLogger();
            var db = new TmdbContext();
            ////db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            var server = new Servers.TMServer(GlobalSettings.AuthPort, GlobalSettings.ApiPort,log);
            server.Start();
            Console.ReadLine();
        }
    }
}