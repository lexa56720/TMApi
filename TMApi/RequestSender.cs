using ApiTypes;
using ApiTypes.Communication.Packets;
using AutoSerializer;
using CSDTP;
using CSDTP.Cryptography.Algorithms;
using CSDTP.Cryptography.Providers;
using CSDTP.Protocols;
using CSDTP.Requests;
using System.Net;

namespace TMApi
{
    public enum RequestKind
    {
        Auth,
        Request,
        LongPoll,
        File
    }

    internal class RequestSender : IDisposable
    {

        public int AuthPort { get; }

        public int ApiPort { get; }

        public int LongPollPort { get; }
        public int FileUploadPort { get; }
        public IPAddress Server { get; }

        public string Token { get; internal set; } = string.Empty;
        public int UserId { get; internal set; }


        private Requester LongPollRequester { get; set; } = null!;
        private Requester Requester { get; set; } = null!;
        private Requester FileRequester { get; set; } = null!;

        private readonly TimeSpan Timeout = TimeSpan.FromSeconds(15);

        private RequestSender(IPAddress server, int authPort, int apiPort, int longPollPort, int fileUploadPort)
        {
            Server = server;
            AuthPort = authPort;
            ApiPort = apiPort;
            LongPollPort = longPollPort;
            FileUploadPort = fileUploadPort;
        }

        private RequestSender(IPAddress server, int authPort, int apiPort, int longPollPort)
        {
            Server = server;
            AuthPort = authPort;
            ApiPort = apiPort;
            LongPollPort = longPollPort;
        }

        internal static async Task<RequestSender> Create(IPAddress server, int authPort, int apiPort, int longPollPort, int fileUploadPort,
                                                         RequestKind kind, IEncryptProvider encryptProvider)
        {
            var requester = new RequestSender(server, authPort, apiPort, longPollPort, fileUploadPort);

            requester.Requester =await RequesterFactory.Create(new IPEndPoint(server, requester.GetPort(kind)),
                                                encryptProvider,typeof(TMPacket<>));

            requester.FileRequester = await RequesterFactory.Create(new IPEndPoint(server, requester.GetPort(RequestKind.File)),
                                                    encryptProvider,typeof(TMPacket<>), Protocol.Http);

            requester.LongPollRequester = await RequesterFactory.Create(new IPEndPoint(server, requester.GetPort(RequestKind.LongPoll)),
                                                        encryptProvider, typeof(TMPacket<>), Protocol.Udp);
            return requester;
        }

        internal static async Task<RequestSender> Create(IPAddress server, int authPort, int apiPort, int longPollPort, RequestKind kind)
        {
            var requester = new RequestSender(server, authPort, apiPort, longPollPort);

            requester.Requester = await RequesterFactory.Create(new IPEndPoint(server, requester.GetPort(kind)), typeof(TMPacket<>));
            requester.LongPollRequester = await RequesterFactory.Create(new IPEndPoint(server, requester.GetPort(RequestKind.LongPoll)),
                                                                        typeof(TMPacket<>),Protocol.Udp);
            return requester;
        }

        public void Dispose()
        {
            Requester.Dispose();
            LongPollRequester.Dispose();
        }


        public async Task<TResponse?> RequestAsync<TResponse, TRequest>(TRequest data)
                                      where TResponse : ISerializable<TResponse>, new()
                                      where TRequest : ISerializable<TRequest>, new()
        {
            return await Requester.RequestAsync<TResponse, TRequest>(data, Timeout);
        }
        public async Task<bool> SendAsync<TRequest>(TRequest data) where TRequest : ISerializable<TRequest>, new()
        {
            return await Requester.SendAsync(data);
        }

        public async Task<TResponse?> ApiRequestAsync<TResponse, TRequest>(TRequest data)
                                      where TResponse : ISerializable<TResponse>, new()
                                      where TRequest : ISerializable<TRequest>, new()
        {
            return await Requester.RequestAsync<TResponse, ApiData<TRequest>>
                         (new ApiData<TRequest>(Token, UserId, data), Timeout);
        }
        public async Task<bool> ApiSendAsync<TData>(TData data) where TData : ISerializable<TData>, new()
        {
            return await Requester.SendAsync(new ApiData<TData>(Token, UserId, data));
        }

        public async Task<bool> SendFileAsync<TData>(TData data) where TData : ISerializable<TData>, new()
        {
            return await FileRequester.SendAsync(new ApiData<TData>(Token, UserId, data));
        }
        public async Task<TResponse?> FileRequestAsync<TResponse, TRequest>(TRequest data)
                                      where TResponse : ISerializable<TResponse>, new()
                                      where TRequest : ISerializable<TRequest>, new()
        {
            return await FileRequester.RequestAsync<TResponse, ApiData<TRequest>>
                         (new ApiData<TRequest>(Token, UserId, data), Timeout);
        }

        public async Task<TResponse?> LongPollAsync<TResponse, TRequest>(TRequest data, TimeSpan timeout, CancellationToken token)
                                      where TResponse : ISerializable<TResponse>, new()
                                      where TRequest : ISerializable<TRequest>, new()
        {
            return await LongPollRequester.RequestAsync<TResponse, ApiData<TRequest>>
                         (new ApiData<TRequest>(Token, UserId, data), timeout, token);
        }

        private int GetPort(RequestKind kind)
        {
            return kind switch
            {
                RequestKind.Auth => AuthPort,
                RequestKind.Request => ApiPort,
                RequestKind.LongPoll => LongPollPort,
                RequestKind.File => FileUploadPort,
                _ => throw new NotImplementedException()
            };
        }
    }
}
