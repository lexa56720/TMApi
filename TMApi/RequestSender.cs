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

        private readonly int CryptId;


        private readonly Requester LongPollRequester;
        private readonly Requester Requester;
        private readonly Requester FileRequester;

        private readonly TimeSpan Timeout = TimeSpan.FromSeconds(15);

        public RequestSender(IPAddress server, int authPort, int apiPort, int longPollPort, int fileUploadPort,
                             RequestKind kind, IEncrypter encrypter, IEncrypter decrypter, int cryptId)
        {
            Server = server;
            AuthPort = authPort;
            ApiPort = apiPort;
            LongPollPort = longPollPort;
            FileUploadPort = fileUploadPort;
            CryptId = cryptId;
            Requester = RequesterFactory.Create(new IPEndPoint(Server, GetPort(kind)),
                                                new SimpleEncryptProvider(encrypter, decrypter),
                                                typeof(TMPacket<>));

            FileRequester = RequesterFactory.Create(new IPEndPoint(Server, GetPort(RequestKind.File)),
                                                    new SimpleEncryptProvider(encrypter, decrypter),
                                                    typeof(TMPacket<>),
                                                    Protocol.Http);

            LongPollRequester = RequesterFactory.Create(new IPEndPoint(Server, GetPort(RequestKind.LongPoll)),
                                                        new SimpleEncryptProvider(encrypter, decrypter),
                                                        typeof(TMPacket<>),
                                                        Protocol.Udp);
        }

        public RequestSender(IPAddress server, int authPort, int apiPort, int longPollPort, RequestKind kind, int cryptId)
        {
            Server = server;
            AuthPort = authPort;
            ApiPort = apiPort;
            LongPollPort = longPollPort;
            CryptId = cryptId;
            Requester = RequesterFactory.Create(new IPEndPoint(Server, GetPort(kind)), typeof(TMPacket<>));
            LongPollRequester = RequesterFactory.Create(new IPEndPoint(Server, GetPort(RequestKind.LongPoll)),
                                                        typeof(TMPacket<>),
                                                        CSDTP.Protocols.Protocol.Udp);
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
            IdHolder.Value = CryptId;
            return await Requester.RequestAsync<TResponse, TRequest>(data, Timeout);
        }
        public async Task<bool> SendAsync<TRequest>(TRequest data) where TRequest : ISerializable<TRequest>, new()
        {
            IdHolder.Value = CryptId;
            return await Requester.SendAsync(data);
        }

        public async Task<TResponse?> ApiRequestAsync<TResponse, TRequest>(TRequest data)
                                      where TResponse : ISerializable<TResponse>, new()
                                      where TRequest : ISerializable<TRequest>, new()
        {
            IdHolder.Value = CryptId;
            return await Requester.RequestAsync<TResponse, ApiData<TRequest>>
                         (new ApiData<TRequest>(Token, UserId, data, IdHolder.Value), Timeout);
        }
        public async Task<bool> ApiSendAsync<TData>(TData data) where TData : ISerializable<TData>, new()
        {
            IdHolder.Value = CryptId;
            return await Requester.SendAsync(new ApiData<TData>(Token, UserId, data, IdHolder.Value));
        }

        public async Task<bool> SendFileAsync<TData>(TData data) where TData : ISerializable<TData>, new()
        {
            IdHolder.Value = CryptId;
            return await FileRequester.SendAsync(new ApiData<TData>(Token, UserId, data, IdHolder.Value));
        }
        public async Task<TResponse?> LongPollAsync<TResponse, TRequest>(TRequest data, TimeSpan timeout, CancellationToken token)
                                      where TResponse : ISerializable<TResponse>, new()
                                      where TRequest : ISerializable<TRequest>, new()
        {
            IdHolder.Value = CryptId;
            return await LongPollRequester.RequestAsync<TResponse, ApiData<TRequest>>
                         (new ApiData<TRequest>(Token, UserId, data, IdHolder.Value), timeout, token);
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
