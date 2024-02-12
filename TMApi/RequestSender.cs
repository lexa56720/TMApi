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
    internal class RequestSender : IDisposable
    {
        internal static int RsaPort { get; set; }

        internal static int AesPort { get; set; }

        internal static int LongPollPort { get; set; }

        internal static IPAddress Server { get; set; } = IPAddress.Loopback;

        public string Token { get; internal set; } = string.Empty;

        public int UserId { get; internal set; }

        public int CryptId => Preferences.CtyptId;

        private Requester LongPollRequester { get; set; }

        private Requester Requester { get; set; }

        public bool IsRsa { get; }

        private TimeSpan Timeout => TimeSpan.FromSeconds(15);

        public RequestSender(bool isRsa, IEncrypter encrypter, IEncrypter decrypter)
        {

            Requester = RequesterFactory.Create(new IPEndPoint(Server, GetPort(isRsa, false)),
                                                new SimpleEncryptProvider(encrypter, decrypter),
                                                typeof(TMPacket<>));

            LongPollRequester = RequesterFactory.Create(new IPEndPoint(Server, GetPort(isRsa, true)),
                                                        new SimpleEncryptProvider(encrypter, decrypter),
                                                        typeof(TMPacket<>));
            IsRsa = isRsa;
        }

        public RequestSender(bool isRsa)
        {
            Requester = RequesterFactory.Create(new IPEndPoint(Server, GetPort(isRsa, false)), typeof(TMPacket<>));
            LongPollRequester = RequesterFactory.Create(new IPEndPoint(Server, GetPort(isRsa, true)), typeof(TMPacket<>));;
            IsRsa = isRsa;
        }

        public void Dispose()
        {
            Requester.Dispose();
            LongPollRequester.Dispose();
        }

        private int GetPort(bool isRsa, bool isLongPoll)
        {
            if (isLongPoll)
                return LongPollPort;
            if (isRsa)
                return RsaPort;
            return AesPort;
        }
        public async Task<TResponse?> PostAsync<TResponse, TRequest>(TRequest data) 
            where TResponse : ISerializable<TResponse>, new()
           where TRequest : ISerializable<TRequest>, new()
        {
            return await Requester.RequestAsync<TResponse, TRequest>(data, Timeout);
        }
        public async Task<bool> GetAsync<TRequest>(TRequest data) where TRequest : ISerializable<TRequest>,new()
        {
            return await Requester.SendAsync(data);
        }

        public async Task<TResponse?> PostAsync<TResponse, TRequest>(RequestHeaders header, TRequest data) 
                              where TResponse : ISerializable<TResponse>,new() 
                              where TRequest : ISerializable<TRequest>, new()
        {
            return await Requester.RequestAsync<TResponse, ApiData<TRequest>>(new ApiData<TRequest>(header, Token, UserId, data, CryptId), Timeout);
        }
        public async Task<bool> GetAsync<T>(RequestHeaders header, T data)where T : ISerializable<T>,new() 
        {
            return await Requester.SendAsync(new ApiData<T>(header, Token, UserId, data, CryptId));
        }

        public async Task<TResponse?> PostAsync<TResponse, TRequest>(RequestHeaders header, TRequest data, TimeSpan timeout)
                              where TResponse : ISerializable<TResponse>, new()
                              where TRequest : ISerializable<TRequest>, new()
        {
            return await Requester.RequestAsync<TResponse, ApiData<TRequest>>
                         (new ApiData<TRequest>(header, Token, UserId, data, CryptId), timeout);
        }

        public async Task<TResponse?> LongPollAsync<TResponse, TRequest>(RequestHeaders header, TRequest data, TimeSpan timeout)
                              where TResponse : ISerializable<TResponse>, new()
                              where TRequest : ISerializable<TRequest>, new()
        {
            return await LongPollRequester.RequestAsync<TResponse, ApiData<TRequest>>
                         (new ApiData<TRequest>(header, Token, UserId, data, CryptId), timeout);
        }
    }
}
