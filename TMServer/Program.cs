using ApiTypes.Communication.BaseTypes;
using Microsoft.Extensions.Logging;
using TMServer.DataBase;
using TMServer.Logger;
using TMServer.ServerComponent;
using TMServer.ServerComponent.Basics;
using TMServer.ServerComponent.Info;
using TMServer.Services;

namespace TMServer
{
    internal class Program
    {
        public static ServerFactory Factory { get; private set; }
        public static ConsoleLogger Logger { get; private set; }

        public Program()
        {

        }

        private static void Main(string[] args)
        {
            Serializer.SerializerProvider = new ApiTypes.SerializerProvider();
            Logger = new ConsoleLogger();
            Factory = new ServerFactory(GlobalSettings.PasswordSalt, Logger);

            DataBaseInit();

            using var mainServer = Create(GlobalSettings.LongPollLifeTime);

            using var infoServer = Create(GlobalSettings.InfoPort, GlobalSettings.Version);
            infoServer.Add(mainServer.ApiServer);
            infoServer.Add(mainServer.AuthServer);
            infoServer.Add(mainServer.LongPollServer);
            infoServer.Add(mainServer.ImageServer);

            infoServer.Start();
            mainServer.Start();

            RunServices();

            Thread.Sleep(1200);
            Logger.Log("\n\n" + new string('*', 10) + "INITIALIZATION OVER" + new string('*', 10));
            Console.ReadLine();
        }

        private static void RunServices()
        {
            var tokenCleaner = new TokenCleaner(TimeSpan.FromMinutes(15), Logger);
            tokenCleaner.Start();

            var keyCleaner = new KeyCleaner(TimeSpan.FromMinutes(15), Logger);
            keyCleaner.Start();
        }

        private static void DataBaseInit()
        {
            using var db = new TmdbContext();
            using var idb = new ImagesDBContext();
            idb.Database.EnsureCreated();
            idb.SaveChanges();
            db.Database.EnsureCreated();
            db.SaveChanges();
        }


        private static InfoServer Create(int port,int version)
        {
            return Factory.CreateInfoServer(port, version);
        }
        private static ServerComponent.TMServer Create(TimeSpan longPollPeriod)
        {
            var authServer = Factory.CreateAuthServer();
            var apiServer = Factory.CreateApiServer();

            var longPollServer = Factory.CreateLongPollServer(longPollPeriod);
            var imageServer = Factory.CreateImageServer();

            return Factory.CreateMainServer(apiServer,authServer,longPollServer,imageServer);
        }
    }
}