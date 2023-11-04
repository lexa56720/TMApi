using ApiTypes;
using ApiTypes.Communication.Packets;
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
            Requester = new Requester(new IPEndPoint(Server, GetPort(isRsa,false)),
                                      new SimpleEncryptProvider(encrypter), 
                                      new SimpleEncryptProvider(decrypter));
            Requester.SetPacketType(typeof(TMPacket<>));

            LongPollRequester = new Requester(new IPEndPoint(Server, GetPort(isRsa, true)),
                                      new SimpleEncryptProvider(encrypter),
                                      new SimpleEncryptProvider(decrypter));
            LongPollRequester.SetPacketType(typeof(TMPacket<>));

            IsRsa = isRsa;
        }

        public RequestSender(bool isRsa)
        {
            Requester = new Requester(new IPEndPoint(Server, GetPort(isRsa, false)));
            Requester.SetPacketType(typeof(TMPacket<>));

            LongPollRequester = new Requester(new IPEndPoint(Server, GetPort(isRsa, true)));
            LongPollRequester.SetPacketType(typeof(TMPacket<>));

            IsRsa = isRsa;
        }

        public void Dispose()
        {
            Requester.Dispose();
        }

        private int GetPort(bool isRsa,bool isLongPoll)
        {
            if (isLongPoll)
                return LongPollPort;
            if (isRsa)
                return RsaPort;
            return AesPort;
        }

        public async Task<T?> PostAsync<T, U>(U data) where T : ISerializable<T> where U : ISerializable<U>
        {
            return await Requester.PostAsync<T, U>(data, Timeout);
        }
        public async Task<bool> GetAsync<T>(T data) where T : ISerializable<T>
        {
            return await Requester.GetAsync<T>(data);
        }

        public async Task<T?> PostAsync<T, U>(RequestHeaders header, U data) where T : ISerializable<T> where U : ISerializable<U>
        {
            return await Requester.PostAsync<T, ApiData<U>>(new ApiData<U>(header, Token, UserId, data, CryptId), Timeout);
        }
        public async Task<bool> GetAsync<T>(RequestHeaders header, T data) where T : ISerializable<T>
        {
            return await Requester.GetAsync(new ApiData<T>(header, Token, UserId, data, CryptId));
        }

        public async Task<T?> PostAsync<T, U>(RequestHeaders header, U data, TimeSpan timeout) 
                                  where T : ISerializable<T> where U : ISerializable<U>
        {
            return await Requester.PostAsync<T, ApiData<U>>
                         (new ApiData<U>(header, Token, UserId, data, CryptId), timeout);
        }

        public async Task<T?> LongPollAsync<T, U>(RequestHeaders header, U data, TimeSpan timeout) 
                                      where T : ISerializable<T> where U : ISerializable<U>
        {
            return await LongPollRequester.PostAsync<T, ApiData<U>>
                         (new ApiData<U>(header, Token, UserId, data, CryptId), timeout);
        }
    }
}
