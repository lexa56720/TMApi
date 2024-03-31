using ApiTypes;
using ApiTypes.Communication.Auth;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Shared;
using CSDTP.Cryptography.Algorithms;
using CSDTP.Packets;
using TMServer.DataBase;
using TMServer.DataBase.Interaction;

namespace TMServer.RequestHandlers
{
    public class AuthHandler
    {
        private readonly Crypt Crypt;
        private readonly LongPolling LongPolling;
        private readonly Authentication Authentication;

        public AuthHandler(Crypt crypt,LongPolling longPolling,Authentication authentication) 
        {
            Crypt = crypt;
            LongPolling = longPolling;
            Authentication = authentication;
        }

        public RsaPublicKey RsaKeyTrade(RsaPublicKey clientKey)
        {
            using var encrypter = new RsaEncrypter();
            var serverKey = encrypter.PublicKey;

            var id = Crypt.SaveRsaKeyPair(encrypter.PrivateKey, clientKey.Key,
                                          DateTime.UtcNow + GlobalSettings.RsaLifeTime);

            return new RsaPublicKey(serverKey, id);
        }

        public AuthorizationResponse Login(AuthorizationRequest request)
        {
            var id = Authentication.GetUserId(request.Login, request.Password);
            if (id < 0)
                return new AuthorizationResponse()
                {
                    IsSuccessful = false
                };
            LongPolling.ClearAllUpdates(id);
            return GetAuthData(id);
        }
        private AuthorizationResponse GetAuthData(int userId)
        {
            var token = HashGenerator.GetRandomString();
            var aesCrypter = new AesEncrypter();
            var expiration = DateTime.UtcNow.Add(GlobalSettings.TokenLifeTime);

            var cryptId = Authentication.SaveAuth(userId, aesCrypter.Key, token, expiration);

            return new AuthorizationResponse()
            {
                AccessToken = token,
                AesKey = aesCrypter.Key,
                Expiration = expiration,
                UserId = userId,
                CryptId = cryptId,
                IsSuccessful = true
            };
        }

        public RegisterResponse Register(RegisterRequest request)
        {
            var isSuccsessful = DataConstraints.IsLoginLegal(request.Login) &&
                                Authentication.IsLoginAvailable(request.Login);

            if (isSuccsessful)
            {
                using var aes = new AesEncrypter();
                Authentication.CreateUser(request.Username, request.Login, request.Password, aes.Key);
            }

            return new RegisterResponse(isSuccsessful);
        }

        public IntContainer GetVersion()
        {
            return new IntContainer(GlobalSettings.Version);
        }

        public AuthorizationResponse UpdateAuth(ApiData<AuthUpdateRequest> request)
        {
            Crypt.SetDeprecated(request.CryptId);
            var newAuth = GetAuthData(request.UserId);
            return newAuth;
        }
    }
}
