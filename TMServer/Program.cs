using ApiTypes.Communication.BaseTypes;
using TMServer.DataBase;
using TMServer.Logger;
using TMServer.Services;

namespace TMServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ILogger logger = new ConsoleLogger();

            var db = new TmdbContext();
            //db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var server = new Servers.TMServer(GlobalSettings.AuthPort, GlobalSettings.ApiPort,logger);
            server.Start();

            var tokenCleaner = new TokenCleaner(TimeSpan.FromMinutes(15),logger);
            tokenCleaner.Start();

            Console.ReadLine();
        }
    }
}