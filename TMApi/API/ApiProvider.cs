using ApiTypes.Communication.Auth;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Info;
using ApiTypes.Communication.Packets;
using ApiTypes.Shared;
using CSDTP.Cryptography.Algorithms;
using CSDTP.Cryptography.Providers;
using CSDTP.Protocols;
using CSDTP.Requests;
using CSDTP.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using TMApi.ApiRequests;
using TMApi.ApiRequests.Security;
using TMApi.Authentication;

namespace TMApi.API
{
    public class ApiProvider
    {
        public required IPAddress Server { get; init; }
        public required int AuthPort { get; init; }
        public required int ApiPort { get; init; }
        public required int LongPollPort { get; init; }
        public required int FileUploadPort { get; init; }
        public required ServerInfo ServerInfo { get; init; }
        public required TimeSpan LongPollPeriod { get; init; }
        public ApiProvider()
        {
        }
        public static async Task<ApiProvider?> CreateProvider(IPAddress serverAddress, int serverPort)
        {
            Serializer.SerializerProvider = new ApiTypes.SerializerProvider();
            using var uncryptRequester = await RequesterFactory.Create(new IPEndPoint(serverAddress, serverPort), Protocol.Udp);
            var response = await uncryptRequester.RequestAsync<ServerInfo, ServerInfoRequest>(new ServerInfoRequest(), TimeSpan.FromSeconds(5));
            if (response == null)
                return null;
            return new ApiProvider()
            {
                Server = serverAddress,
                AuthPort = response.AuthPort,
                ApiPort = response.ApiPort,
                FileUploadPort = response.FileUploadPort,
                LongPollPort = response.LongPollPort,
                ServerInfo = response,
                LongPollPeriod = TimeSpan.FromSeconds(response.LongPollPeriodSeconds),
            };
        }

        public async Task<Api?> GetApiRegistration(string username, string login, string password)
        {
            using var rsaEncryptProvider = await GetCoderDecoder();
            if (rsaEncryptProvider == null)
                return null;

            using var rsaRequester = await RequestSender.Create(Server, AuthPort, ApiPort,
                LongPollPort, FileUploadPort, RequestKind.Auth, rsaEncryptProvider);

            RegistrationResponse? registerResult = await rsaRequester.RequestAsync<RegistrationResponse, RegistrationRequest>(new RegistrationRequest()
            {
                Username = username,
                Login = login,
                Password = HashGenerator.GetPasswordHash(password, login),
            });


            if (registerResult == null || !registerResult.IsAccepted)
                return null;

            var authResult = await Auth.Login(login, password, rsaRequester);
            if (authResult == null || !authResult.IsSuccessful)
                return null;

            return await GetApi(authResult.AccessToken, authResult.Expiration,
                                authResult.UserId, authResult.CryptId, authResult.AesKey);
        }

        public async Task<Api?> GetApiLogin(string login, string password)
        {
            using var rsaEncryptProvider = await GetCoderDecoder();
            if (rsaEncryptProvider == null)
                return null;
            using var rsaRequester = await RequestSender.Create(Server, AuthPort, ApiPort,
                 LongPollPort, FileUploadPort, RequestKind.Auth, rsaEncryptProvider);
            var authResult = await Auth.Login(login, password, rsaRequester);
            if (authResult == null || !authResult.IsSuccessful)
                return null;

            return await GetApi(authResult.AccessToken, authResult.Expiration,
                                authResult.UserId, authResult.CryptId, authResult.AesKey);
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
            if (await api.Init(LongPollPeriod, Server, AuthPort, ApiPort, LongPollPort, FileUploadPort))
                return api;
            return null;
        }
        private async Task<AuthEncryptProvider?> GetCoderDecoder()
        {
            try
            {
                var inputDecoder = new RsaEncrypter();
                using var uncryptRequester = await RequestSender.Create(Server, AuthPort, ApiPort, LongPollPort, RequestKind.Auth);
                var (publicKey, id) = await Auth.GetRsaKey(uncryptRequester, inputDecoder);
                var outputEncoder = new RsaEncrypter(publicKey);

                return new AuthEncryptProvider(outputEncoder, inputDecoder, id);
            }
            catch
            {
                return null;
            }
        }
    }
}
