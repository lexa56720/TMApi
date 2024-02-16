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
            Serializer.SerializerProvider = new ApiTypes.SerializerProvider();

            ILogger logger = new ConsoleLogger();

            using var db = new TmdbContext();
           // db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            db.SaveChanges();

            using var server = new Servers.TMServer(GlobalSettings.AuthPort,
                                                    GlobalSettings.ApiPort,
                                                    GlobalSettings.LongPollPort,
                                                    logger);
            server.Start();

            var longPollCleaner = new LongPollCleaner(GlobalSettings.LongPollLifeTime, TimeSpan.FromMinutes(3), logger);
            longPollCleaner.Start();

            var tokenCleaner = new TokenCleaner(TimeSpan.FromMinutes(15),logger);
            tokenCleaner.Start();

            var keyCleaner=new KeyCleaner(TimeSpan.FromMinutes(15),logger); 
            keyCleaner.Start();

            Thread.Sleep(1000);
            logger.Log("\n\n" + new string('*', 10) + "INITIALIZATION OVER" + new string('*', 10));
            Console.ReadLine();
        }
    }
}