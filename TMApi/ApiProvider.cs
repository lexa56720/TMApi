using ApiTypes.Communication.Auth;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Info;
using ApiTypes.Communication.Packets;
using ApiTypes.Shared;
using CSDTP.Cryptography.Algorithms;
using System.Net;

namespace TMApi
{
    public class ApiProvider
    {
        public ApiProvider(IPAddress server, int authPort, int apiPort,int longPollPort)
        {
            RequestSender.Server = server;
            RequestSender.RsaPort = authPort;
            RequestSender.AesPort = apiPort;
            RequestSender.LongPollPort = longPollPort;
        }

        public async Task<int> GetVersion()
        {
            using var uncryptRequester = new RequestSender(true);
            var version = await uncryptRequester.PostAsync<IntContainer, VersionRequest>
                (new VersionRequest());

            if (version == null)
                return -1;
            return version.Value;
        }
        public async Task<Api?> GetApiRegistration(string username, string login, string password)
        {
            Preferences.CtyptId = 0;
            var coderDecoder = await GetCoderDecoder();
            if (coderDecoder == null)
                return null;
            using var inputDecoder = coderDecoder.Value.Item1;
            using var outputEncoder = coderDecoder.Value.Item2;

            RequestResponse? registerResult = null;
            using var rsaRequester = new RequestSender(true, outputEncoder, inputDecoder);

            registerResult = await rsaRequester.PostAsync<RequestResponse, RegisterRequest>(new RegisterRequest()
            {
                Username = username,
                Login = login,
                Password = GetPasswordHash(password),
            });


            if (registerResult !=null && registerResult.IsAccepted)
                return await Login(login, password, inputDecoder, outputEncoder);

            return null;
        }

        public async Task<Api?> GetApiLogin(string login, string password)
        {
            Preferences.CtyptId = 0;
            var coderDecoder = await GetCoderDecoder();
            if (coderDecoder == null)
                return null;
            using var inputDecoder = coderDecoder.Value.decoder;
            using var outputEncoder = coderDecoder.Value.encoder;

            return await Login(login, password, inputDecoder, outputEncoder);
        }

        private async Task<Api?> Login(string login, string password, RsaEncrypter inputDecoder, RsaEncrypter outputEncoder)
        {
            password = GetPasswordHash(password);
            AuthorizationResponse? authResult = null;
            using (var rsaRequester = new RequestSender(true, outputEncoder, inputDecoder))
            {
                authResult = await rsaRequester.PostAsync<AuthorizationResponse, AuthorizationRequest>(new AuthorizationRequest(login, password));
            }

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
            var api = new Api(token, expiration, userId, cryptId, aesKey);
            if (await api.Init())
                return api;
            return null;
        }
        private async Task<(RsaEncrypter decoder, RsaEncrypter encoder)?> GetCoderDecoder()
        {
            try
            {
                var inputDecoder = new RsaEncrypter();

                var (publicKey, id) = await GetRsaKey(inputDecoder);
                var outputEncoder = new RsaEncrypter(publicKey);

                Preferences.CtyptId= id;

                return (inputDecoder, outputEncoder);
            }
            catch
            {
                return null;
            }

        }
        private async Task<(string publicKey, int id)> GetRsaKey(RsaEncrypter inputDecoder)
        {
            using var uncryptRequester = new RequestSender(true);
            var request = new RsaPublicKey(inputDecoder.PublicKey);
            var response = await uncryptRequester.PostAsync<RsaPublicKey, RsaPublicKey>(request)
                ?? throw new Exception("no response");

            string serverRsaPublicKey = response.Key;
            return (serverRsaPublicKey, response.Id);
        }

        private string GetPasswordHash(string password)
        {
            return HashGenerator.GenerateHash(password);
        }
    }
}
