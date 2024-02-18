using ApiTypes;
using ApiTypes.Communication.Packets;
using AutoSerializer;
using CSDTP;
using CSDTP.Cryptography.Algorithms;
using CSDTP.Cryptography.Providers;
using CSDTP.Requests;
using System.Net;

namespace TMApi
{
    public enum RequestKind
    {
        Auth,
        Request,
        LongPoll
    }

    internal class RequestSender : IDisposable
    {
        public int AuthPort { get; init; }

        public int ApiPort { get; init; }

        public int LongPollPort { get; init; }

        public required IPAddress Server { get; init; }

        public string Token { get; internal set; } = string.Empty;

        public int UserId { get; internal set; }

        private Requester LongPollRequester { get; set; }
        private Requester Requester { get; set; }

        private TimeSpan Timeout => TimeSpan.FromSeconds(15);

        public RequestSender(RequestKind kind, IEncrypter encrypter, IEncrypter decrypter)
        {

            Requester = RequesterFactory.Create(new IPEndPoint(Server, GetPort(kind)),
                                                new SimpleEncryptProvider(encrypter, decrypter),
                                                typeof(TMPacket<>));

            LongPollRequester = RequesterFactory.Create(new IPEndPoint(Server, GetPort(kind)),
                                                        new SimpleEncryptProvider(encrypter, decrypter),
                                                        typeof(TMPacket<>));
        }

        public RequestSender(RequestKind kind)
        {
            Requester = RequesterFactory.Create(new IPEndPoint(Server, GetPort(kind)), typeof(TMPacket<>));
            LongPollRequester = RequesterFactory.Create(new IPEndPoint(Server, GetPort(kind)), typeof(TMPacket<>));
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

        public async Task<TResponse?> RequestAsync<TResponse, TRequest>(RequestHeaders header, TRequest data)
                                      where TResponse : ISerializable<TResponse>, new()
                                      where TRequest : ISerializable<TRequest>, new()
        {
            return await Requester.RequestAsync<TResponse, ApiData<TRequest>>(new ApiData<TRequest>(header, Token, UserId, data, IdHolder.Value), Timeout);
        }
        public async Task<bool> SendAsync<T>(RequestHeaders header, T data) where T : ISerializable<T>, new()
        {
            return await Requester.SendAsync(new ApiData<T>(header, Token, UserId, data, IdHolder.Value));
        }

        public async Task<TResponse?> RequestAsync<TResponse, TRequest>(RequestHeaders header, TRequest data, TimeSpan timeout)
                                      where TResponse : ISerializable<TResponse>, new()
                                      where TRequest : ISerializable<TRequest>, new()
        {
            return await Requester.RequestAsync<TResponse, ApiData<TRequest>>
                         (new ApiData<TRequest>(header, Token, UserId, data, IdHolder.Value), timeout);
        }

        public async Task<TResponse?> LongPollAsync<TResponse, TRequest>(RequestHeaders header, TRequest data, TimeSpan timeout)
                                      where TResponse : ISerializable<TResponse>, new()
                                      where TRequest : ISerializable<TRequest>, new()
        {
            return await LongPollRequester.RequestAsync<TResponse, ApiData<TRequest>>
                         (new ApiData<TRequest>(header, Token, UserId, data, IdHolder.Value), timeout);
        }

        private int GetPort(RequestKind kind)
        {
            return kind switch
            {
                RequestKind.Auth => AuthPort,
                RequestKind.Request => ApiPort,
                RequestKind.LongPoll => LongPollPort,
                _ => throw new NotImplementedException()
            };
        }
    }
}
