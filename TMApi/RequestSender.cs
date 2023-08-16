using ApiTypes;
using CSDTP;
using CSDTP.Cryptography;
using CSDTP.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TMApi
{
    internal class RequestSender : IDisposable
    {

        private const int RsaPort = 6665;

        private const int AesPort = 6666;

        private IPAddress Server { get; set; } = new IPAddress(new byte[] { 127, 0, 0, 1 });

        public string Token { get; set; }

        public int Id { get; set; }

        Requester Requester { get; set; }

        public bool IsRsa { get; }

        private TimeSpan Timeout => TimeSpan.FromSeconds(5);

        public RequestSender(bool isRsa, IEncrypter encrypter)
        {
            Requester = new Requester(new IPEndPoint(Server, GetPort(isRsa)), encrypter);
            IsRsa = isRsa;
        }
        public RequestSender(bool isRsa, IEncrypter encrypter, IEncrypter decrypter)
        {
            Requester = new Requester(new IPEndPoint(Server, GetPort(isRsa)), encrypter, decrypter);
            IsRsa = isRsa;
        }

        public RequestSender(bool isRsa)
        {
            Requester = new Requester(new IPEndPoint(Server, GetPort(isRsa)));
            IsRsa = isRsa;
        }

        private int GetPort(bool isRsa)
        {
            if (isRsa)
                return RsaPort;
            return AesPort;
        }
        public async Task<T> PostAsync<T, U>(U data) where T : ISerializable<T> where U : ISerializable<U>
        {
            return await Requester.PostAsync<T, U>(data, Timeout);
        }
        public async Task<bool> GetAsync<T>(T data) where T : ISerializable<T>
        {
            return await Requester.GetAsync<T>(data);
        }


        public async Task<T> PostRequestAsync<T, U>(U data) where T : ISerializable<T> where U : ISerializable<U>
        {
            return await Requester.PostAsync<T, ApiRequest<U>>(new ApiRequest<U>(Token,Id, data), Timeout);
        }
        public async Task<bool> GetRequestAsync<T>(T data) where T : ISerializable<T>
        {
            return await Requester.GetAsync(new ApiRequest<T>(Token, Id, data));
        }
        public void Dispose()
        {
            Requester.Dispose();
        }
    }
}
