using ApiTypes.Communication.Info;
using CSDTP.Cryptography.Algorithms;
using CSDTP.Cryptography.Providers;
using CSDTP.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Interaction;
using TMServer.Logger;
using TMServer.ServerComponent.Api;
using TMServer.ServerComponent.Auth;
using TMServer.ServerComponent.Basics;
using TMServer.ServerComponent.Files;
using TMServer.ServerComponent.LongPolling;

namespace TMServer.ServerComponent.Info
{
    internal class InfoServer : Server
    {
        private readonly IList<AuthorizationServer> AuthServers = new List<AuthorizationServer>();
        private readonly IList<ApiServer> ApiServers = new List<ApiServer>();
        private readonly IList<LongPollServer> LongPollServers = new List<LongPollServer>();
        private readonly IList<FileServer> FileServers = new List<FileServer>();


        public required int Version { get; init; }
        public required int MaxAttachments { get; init; }
        public required int MaxFileSizeMB { get; init; }

        public InfoServer(int port, ILogger logger) : base(port, logger, CSDTP.Protocols.Protocol.Udp)
        {
            Responder.RegisterRequestHandler<ServerInfoRequest, ServerInfo>(GetInfo);
        }
        public override void Dispose()
        {
            base.Dispose();
            AuthServers.Clear();
            ApiServers.Clear();
            LongPollServers.Clear();
            FileServers.Clear();
        }

        public bool Add(Server server)
        {
            if (server is FileServer imageServer)
            {
                FileServers.Add(imageServer);
                return true;
            }
            switch (server)
            {
                case AuthorizationServer:
                    AuthServers.Add((AuthorizationServer)server);
                    return true;
                case ApiServer:
                    ApiServers.Add((ApiServer)server);
                    return true;
                case LongPollServer:
                    LongPollServers.Add((LongPollServer)server);
                    return true;
            }

            return false;
        }

        private ServerInfo? GetInfo(ServerInfoRequest request, IPacketInfo info)
        {
            Logger.Log($"inf request from {info.Source?.ToString()}");
            var (longPollPort, longPollPeriod) = GetLongPollInfo();
            var (uploadPort, downloadPort) = GetFilePorts();
            var apiPort = GetApiPort();
            var authPort = GetAuthPort();
            if (longPollPort < 0 || longPollPeriod < 0 || uploadPort < 0 ||
                downloadPort < 0 || apiPort < 0 || authPort < 0)
            {
                return null;
            }

            return new ServerInfo()
            {
                AuthPort = authPort,
                ApiPort = apiPort,
                LongPollPort = longPollPort,
                LongPollPeriodSeconds = longPollPeriod,
                FileUploadPort = uploadPort,
                FileGetPort = downloadPort,
                MaxAttachments= MaxAttachments,
                MaxFileSizeMB= MaxFileSizeMB,
                Version = Version,
            };
        }
        private int GetAuthPort()
        {
            var auth = Pick(AuthServers);
            if (auth == null)
                return -1;
            return auth.ListenPort;
        }
        private int GetApiPort()
        {
            var api = Pick(ApiServers);
            if (api == null)
                return -1;
            return api.ListenPort;
        }
        private (int port, int period) GetLongPollInfo()
        {
            var longPoll = Pick(LongPollServers);
            if (longPoll == null)
                return (-1, -1);
            return (longPoll.ListenPort, (int)longPoll.LongPollLifetime.TotalSeconds);
        }
        private (int uploadPort, int downloadPort) GetFilePorts()
        {
            var image = Pick(FileServers);
            if (image == null)
                return (-1, -1);
            return (image.ListenPort, image.DownloadPort);
        }
        private T? Pick<T>(IList<T> list)
        {
            if (list.Count > 0)
                return list[0];
            return default;
        }
    }
}
