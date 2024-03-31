using ApiTypes.Communication.BaseTypes;
using TMServer.DataBase;
using TMServer.Logger;
using TMServer.ServerComponent;
using TMServer.ServerComponent.Basics;
using TMServer.Services;

namespace TMServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Serializer.SerializerProvider = new ApiTypes.SerializerProvider();

            var logger = new ConsoleLogger();

            using var db = new TmdbContext();
            using var idb = new ImagesDBContext();

            idb.Database.EnsureCreated();
            idb.SaveChanges();

            db.Database.EnsureCreated();
            db.SaveChanges();

            using var server = Create(logger);
            server.Start();

            var tokenCleaner = new TokenCleaner(TimeSpan.FromMinutes(15), logger);
            tokenCleaner.Start();

            var keyCleaner = new KeyCleaner(TimeSpan.FromMinutes(15), logger);
            keyCleaner.Start();

            Thread.Sleep(1000);
            logger.Log("\n\n" + new string('*', 10) + "INITIALIZATION OVER" + new string('*', 10));
            Console.ReadLine();
        }

        private static ServerComponent.TMServer Create(ILogger logger)
        {
            var factory = new ServerFactory(GlobalSettings.PasswordSalt, logger);

            var authServer = factory.CreateAuthServer(GlobalSettings.AuthPort);
            var apiServer = factory.CreateApiServer(GlobalSettings.ApiPort);

            var longPollServer = factory.CreateLongPollServer(GlobalSettings.LongPollPort, GlobalSettings.LongPollLifeTime);
            var imageServer = factory.CreateImageServer(GlobalSettings.FileUploadPort, GlobalSettings.FileGetPort);

            return factory.CreateMainServer(apiServer,authServer,longPollServer,imageServer);
        }
    }
}