using ApiTypes.Communication.Auth;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Info;
using ApiTypes.Communication.Packets;
using ApiTypes.Shared;
using CSDTP.Cryptography.Algorithms;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace TMApi
{
    public class ApiProvider
    {
        public required IPAddress Server { get; init; }
        public required int AuthPort { get; init; }
        public required int ApiPort { get; init; }
        public required int LongPollPort { get; init; }
        public required int ImageUploadPort { get; init; }
        public required TimeSpan LongPollPeriod { get; init; }

        [SetsRequiredMembers]
        public ApiProvider(IPAddress server, int authPort, int apiPort, int longPollPort,int imageUploadPort, TimeSpan longPollPeriod)
        {
            Serializer.SerializerProvider = new ApiTypes.SerializerProvider();
            Server = server;
            AuthPort = authPort;
            ApiPort = apiPort;
            LongPollPort = longPollPort;
            ImageUploadPort = imageUploadPort;
            LongPollPeriod = longPollPeriod;
        }

        public ApiProvider()
        {
            Serializer.SerializerProvider = new ApiTypes.SerializerProvider();
        }

        public async Task<int> GetVersion()
        {
            using var uncryptRequester = new RequestSender(Server, AuthPort, ApiPort, LongPollPort, RequestKind.Auth,0);
            var version = await uncryptRequester.RequestAsync<IntContainer, VersionRequest>
                (new VersionRequest());

            if (version == null)
                return -1;
            return version.Value;
        }
        public async Task<Api?> GetApiRegistration(string username, string login, string password)
        {
            var coderDecoder = await GetCoderDecoder();
            if (coderDecoder == null)
                return null;
            using var inputDecoder = coderDecoder.Value.decoder;
            using var outputEncoder = coderDecoder.Value.encoder;

            using var rsaRequester = new RequestSender(Server, AuthPort, ApiPort, LongPollPort, ImageUploadPort, RequestKind.Auth,
                                                                          outputEncoder, inputDecoder,coderDecoder.Value.cryptId);


            RegisterResponse? registerResult = await rsaRequester.RequestAsync<RegisterResponse, RegisterRequest>(new RegisterRequest()
            {
                Username = username,
                Login = login,
                Password = GetPasswordHash(password,login),
            });


            if (registerResult != null && registerResult.IsAccepted)
                return await Login(login, password, inputDecoder, outputEncoder, coderDecoder.Value.cryptId);

            return null;
        }

        public async Task<Api?> GetApiLogin(string login, string password)
        {
            var coderDecoder = await GetCoderDecoder();
            if (coderDecoder == null)
                return null;
            using var inputDecoder = coderDecoder.Value.decoder;
            using var outputEncoder = coderDecoder.Value.encoder;

            return await Login(login, password, inputDecoder, outputEncoder,coderDecoder.Value.cryptId);
        }

        private async Task<Api?> Login(string login, string password, RsaEncrypter inputDecoder, RsaEncrypter outputEncoder,int cryptId)
        {
            password = GetPasswordHash(password,login);
            var rsaRequester = new RequestSender(Server, AuthPort, ApiPort, LongPollPort, ImageUploadPort,
                                                   RequestKind.Auth, outputEncoder, inputDecoder, cryptId);

            var authResult = await rsaRequester.RequestAsync<AuthorizationResponse, AuthorizationRequest>
                                                              (new AuthorizationRequest(login, password));
            rsaRequester.Dispose();

            if (authResult != null && authResult.IsSuccessful)
                return await GetApi(authResult.AccessToken, authResult.Expiration,
                                    authResult.UserId, authResult.CryptId, authResult.AesKey);
            return null;
        }

        public async Task<Api?> DeserializeAuthData(byte[] authData)
        {
            using var ms = new MemoryStream(authData);
            using var br = new BinaryReader(ms);

            var token = br.ReadString();
            var expiration = DateTime.FromBinary(br.ReadInt64());
            var key = br.ReadBytes(32);
            var cryptId = br.ReadInt32();
            var id = br.ReadInt32();

            if (expiration < DateTime.UtcNow.AddHours(1))
                return null;


            var api = await GetApi(token, expiration, id, cryptId, key);
            if (api == null)
                return null;

            var newData = await api.Auth.UpdateAuth();

            if (newData != null && newData.IsSuccessful)
                api.UpdateApiData(newData);
            return api;
        }

        private async Task<Api?> GetApi(string token, DateTime expiration, int userId, int cryptId, byte[] aesKey)
        {
            var api = new Api(token, expiration, userId, cryptId, aesKey,
                              Server, AuthPort, ApiPort, LongPollPort, ImageUploadPort);
            if (await api.Init(LongPollPeriod))
                return api;
            return null;
        }
        private async Task<(RsaEncrypter decoder, RsaEncrypter encoder,int cryptId)?> GetCoderDecoder()
        {
            try
            {
                var inputDecoder = new RsaEncrypter();

                var (publicKey, id) = await GetRsaKey(inputDecoder);
                var outputEncoder = new RsaEncrypter(publicKey);

                return (inputDecoder, outputEncoder, id);
            }
            catch
            {
                return null;
            }
        }
        private async Task<(string publicKey, int id)> GetRsaKey(RsaEncrypter inputDecoder)
        {
            using var uncryptRequester = new RequestSender(Server, AuthPort, ApiPort, LongPollPort, RequestKind.Auth, 0);
            var request = new RsaPublicKey(inputDecoder.PublicKey);
            var response = await uncryptRequester.RequestAsync<RsaPublicKey, RsaPublicKey>(request)
                ?? throw new Exception("no response");

            string serverRsaPublicKey = response.Key;
            return (serverRsaPublicKey, response.Id);
        }

        private string GetPasswordHash(string password,string login)
        {
            return HashGenerator.GenerateHash(password+login);
        }
    }
}
