using ApiTypes.Communication.BaseTypes;
using TMServer.DataBase;
using TMServer.Logger;
using TMServer.ServerComponent;
using TMServer.ServerComponent.Basics;
using TMServer.ServerComponent.Info;

namespace TMServer
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Serializer.SerializerProvider = new ApiTypes.SerializerProvider();
            DataBaseInit();

            var logger = CreateLogger();
            var factory = CreateFactory(logger);
            var (main, info) = CreateServers(factory);

            await main.Start();
            await info.Start();

            ServerRunned(logger);
            await CommandParsingLoop(logger, main, info);
            ServerShutdown(logger);
        }



        private static async Task CommandParsingLoop(ILogger logger, params Startable[] servers)
        {
            while (true)
            {
                var command = Console.ReadLine();
                switch (command)
                {
                    case "help":
                        Console.WriteLine("Existing commands: help, start, stop, restart, exit.");
                        break;
                    case "stop":
                        await Task.WhenAll(servers.Select(s => s.Stop()));
                        ServerStopped(logger);
                        break;
                    case "restart":
                        await Task.WhenAll(servers.Select(s => s.Stop()));
                        ServerStopped(logger);
                        await Task.WhenAll(servers.Select(s => s.Start()));
                        ServerRunned(logger);
                        break;
                    case "start":
                        await Task.WhenAll(servers.Select(s => s.Start()));
                        ServerRunned(logger);
                        break;
                    case "exit":
                        await Task.WhenAll(servers.Select(s => s.Stop()));
                        ServerStopped(logger);
                        return;
                }
            }
        }

        private static void ServerRunned(ILogger logger)
        {
            logger.LogEmptyLine();
            logger.Log(new string('*', 10) + "SERVER IS RUNNING" + new string('*', 10));
        }
        private static void ServerStopped(ILogger logger)
        {
            logger.LogEmptyLine();
            logger.Log(new string('*', 10) + "SERVER IS STOPPED" + new string('*', 10));
        }
        private static void ServerShutdown(ILogger logger)
        {
            logger.LogEmptyLine();
            logger.Log(new string('*', 10) + "SERVER IS SHUTDOWN" + new string('*', 10));
        }

        private static void DataBaseInit()
        {
            using var mainDB = new TmdbContext();
            using var filesDB = new FilesDBContext();

            filesDB.Database.EnsureCreated();
            filesDB.SaveChanges();

            mainDB.Database.EnsureCreated();
            mainDB.SaveChanges();
        }
        private static ILogger CreateLogger()
        {
            return new ConsoleLogger();
        }
        private static ServerFactory CreateFactory(ILogger logger)
        {
            return new ServerFactory(Settings.TokenLifeTime,
                                        Settings.OnlineTimeout,
                                        Settings.RsaLifeTime,
                                        Settings.AesLifeTime,
                                        Settings.PasswordSalt,
                                        Settings.FilesFolder,
                                        Settings.ImagesFolder,
                                        Settings.MaxFileSizeMB,
                                        Settings.MaxAttachments,
                                        logger);
        }
        private static (MainServer main, InfoServer info) CreateServers(ServerFactory factory)
        {
            var mainServer = CreateMainServer(factory,
                                    Settings.LongPollLifeTime,
                                    Settings.AuthPort,
                                    Settings.ApiPort,
                                    Settings.LongPollPort,
                                    Settings.FileDownloadPort,
                                    Settings.FileUploadPort);

            var infoServer = CreateInfoServer(factory, Settings.InfoPort, Settings.Version);
            infoServer.Add(mainServer.ApiServer);
            infoServer.Add(mainServer.AuthServer);
            infoServer.Add(mainServer.LongPollServer);
            infoServer.Add(mainServer.FileServer);

            return (mainServer, infoServer);
        }
        private static InfoServer CreateInfoServer(ServerFactory factory, int port, int version)
        {
            return factory.CreateInfoServer(port, version);
        }
        private static MainServer CreateMainServer(ServerFactory factory, TimeSpan longPollPeriod, int authPort, int apiPort, int longPollPort, int fileGetPort, int fileUploadPort)
        {
            var authServer = factory.CreateAuthServer(authPort);
            var apiServer = factory.CreateApiServer(apiPort);

            var longPollServer = factory.CreateLongPollServer(longPollPort, longPollPeriod);
            var fileServer = factory.CreateFileServer(fileUploadPort, fileGetPort);

            return factory.CreateMainServer(apiServer, authServer, longPollServer, fileServer);
        }
    }
}