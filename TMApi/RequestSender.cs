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

        private const int RsaPort = 6665;

        private const int AesPort = 6666;

        private IPAddress Server => new IPAddress(new byte[] { 127, 0, 0, 1 });

        public string Token { get; internal set; } = string.Empty;

        public int UserId { get; internal set; }

        public int CryptId => IdHolder.Value;

        private Requester Requester { get; set; }

        public bool IsRsa { get; }

        private TimeSpan Timeout => TimeSpan.FromSeconds(50);

        public RequestSender(bool isRsa, IEncrypter encrypter, IEncrypter decrypter)
        {
            Requester = new Requester(new IPEndPoint(Server, GetPort(isRsa)), new SimpleEncryptProvider(encrypter), new SimpleEncryptProvider(decrypter));
            Requester.SetPacketType(typeof(TMPacket<>));
            IsRsa = isRsa;
        }

        public RequestSender(bool isRsa)
        {
            Requester = new Requester(new IPEndPoint(Server, GetPort(isRsa)));
            Requester.SetPacketType(typeof(TMPacket<>));
            IsRsa = isRsa;
        }
        public void Dispose()
        {
            Requester.Dispose();
        }

        private int GetPort(bool isRsa)
        {
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


        public async Task<T?> PostRequestAsync<T, U>(U data) where T : ISerializable<T> where U : ISerializable<U>
        {
            return await Requester.PostAsync<T, ApiData<U>>(new ApiData<U>(Token, UserId, data), Timeout);
        }
        public async Task<bool> GetRequestAsync<T>(T data) where T : ISerializable<T>
        {
            return await Requester.GetAsync(new ApiData<T>(Token, UserId, data));
        }

        public async Task<T?> PostRequestAsync<T, U>(RequestHeaders header, U data) where T : ISerializable<T> where U : ISerializable<U>
        {
            return await Requester.PostAsync<T, ApiData<U>>(new ApiData<U>(header, Token, UserId, data, CryptId), Timeout);
        }
        public async Task<bool> GetRequestAsync<T>(RequestHeaders header, T data) where T : ISerializable<T>
        {
            return await Requester.GetAsync(new ApiData<T>(header, Token, UserId, data, CryptId));
        }

        public async Task<T?> PostRequestAsync<T, U>(RequestHeaders header, U data, TimeSpan timeout) where T : ISerializable<T> where U : ISerializable<U>
        {
            return await Requester.PostAsync<T, ApiData<U>>(new ApiData<U>(header, Token, UserId, data, CryptId), timeout);
        }
    }
}
