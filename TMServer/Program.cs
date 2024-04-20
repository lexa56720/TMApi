using ApiTypes.Communication.BaseTypes;
using Microsoft.Extensions.Logging;
using TMServer.DataBase;
using TMServer.Logger;
using TMServer.ServerComponent;
using TMServer.ServerComponent.Basics;
using TMServer.ServerComponent.Info;

namespace TMServer
{
    internal class Program
    {
        public static ServerFactory Factory { get; private set; }
        public static ConsoleLogger Logger { get; private set; }


        private static void Main(string[] args)
        {
            Serializer.SerializerProvider = new ApiTypes.SerializerProvider();
            Logger = new ConsoleLogger();
            Factory = new ServerFactory(Settings.TokenLifeTime,
                                        Settings.OnlineTimeout,
                                        Settings.RsaLifeTime,
                                        Settings.AesLifeTime,
                                        Settings.PasswordSalt,
                                        Settings.FilesFolder,
                                        Settings.ImagesFolder,
                                        Settings.MaxFileSizeMB,
                                        Settings.MaxAttachments,
                                        Logger);

            DataBaseInit();

            using var mainServer = Create(Settings.LongPollLifeTime,
                                          Settings.AuthPort,
                                          Settings.ApiPort,
                                          Settings.LongPollPort,
                                          Settings.FileDownloadPort,
                                          Settings.FileUploadPort);

            using var infoServer = Create(Settings.InfoPort, Settings.Version);
            infoServer.Add(mainServer.ApiServer);
            infoServer.Add(mainServer.AuthServer);
            infoServer.Add(mainServer.LongPollServer);
            infoServer.Add(mainServer.FileServer);

            infoServer.Start();
            mainServer.Start();

            Thread.Sleep(1500);
            Logger.Log("\n\n" + new string('*', 10) + "INITIALIZATION OVER" + new string('*', 10));
            Console.ReadLine();
        }


        private static void DataBaseInit()
        {
            using var db = new TmdbContext();
            using var idb = new FilesDBContext();
            idb.Database.EnsureCreated();
            idb.SaveChanges();
            db.Database.EnsureCreated();
            db.SaveChanges();
        }


        private static InfoServer Create(int port, int version)
        {
            return Factory.CreateInfoServer(port, version);
        }
        private static ServerComponent.TMServer Create(TimeSpan longPollPeriod, int authPort, int apiPort, int longPollPort, int fileGetPort, int fileUploadPort)
        {
            var authServer = Factory.CreateAuthServer(authPort);
            var apiServer = Factory.CreateApiServer(apiPort);

            var longPollServer = Factory.CreateLongPollServer(longPollPort, longPollPeriod);
            var fileServer = Factory.CreateFileServer(fileUploadPort, fileGetPort);

            return Factory.CreateMainServer(apiServer, authServer, longPollServer, fileServer);
        }
    }
}