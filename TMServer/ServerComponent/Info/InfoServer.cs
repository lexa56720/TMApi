﻿using ApiTypes.Communication.Info;
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
using TMServer.ServerComponent.Images;
using TMServer.ServerComponent.LongPolling;

namespace TMServer.ServerComponent.Info
{
    internal class InfoServer : Server
    {
        private readonly IList<AuthorizationServer> AuthServers = new List<AuthorizationServer>();
        private readonly IList<ApiServer> ApiServers = new List<ApiServer>();
        private readonly IList<LongPollServer> LongPollServers = new List<LongPollServer>();
        private readonly IList<ImageServer> ImageServers = new List<ImageServer>();
        public int Version { get; }

        public InfoServer(int port, int version, ILogger logger) : base(port, logger, CSDTP.Protocols.Protocol.Udp)
        {
            Responder.RegisterRequestHandler<ServerInfoRequest, ServerInfoResponse>(GetInfo);
            Version = version;
        }
        public override void Dispose()
        {
            base.Dispose();
            AuthServers.Clear();
            ApiServers.Clear();
            LongPollServers.Clear();
            ImageServers.Clear();
        }

        public bool Add(Server server)
        {
            if (server is ImageServer imageServer)
            {
                ImageServers.Add(imageServer);
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

        private ServerInfoResponse? GetInfo(ServerInfoRequest request, IPacketInfo info)
        {
            var (longPollPort, longPollPeriod) = GetLongPollInfo();
            var (uploadPort, downloadPort) = GetImagePorts();
            var apiPort = GetApiPort();
            var authPort = GetAuthPort();
            if (longPollPort < 0 || longPollPeriod < 0 || uploadPort < 0 ||
                downloadPort < 0 || apiPort < 0 || authPort < 0)
            {
                return null;
            }

            return new ServerInfoResponse()
            {
                AuthPort = authPort,
                ApiPort = apiPort,
                LongPollPort = longPollPort,
                LongPollPeriodSeconds = longPollPeriod,
                FileUploadPort = uploadPort,
                FileGetPort = downloadPort,
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
        private (int uploadPort, int downloadPort) GetImagePorts()
        {
            var image = Pick(ImageServers);
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