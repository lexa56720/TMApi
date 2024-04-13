using ApiTypes;
using ApiTypes.Communication.Auth;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Users;
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
        private readonly Security Security;
        public AuthHandler(Crypt crypt, LongPolling longPolling, Security security, Authentication authentication)
        {
            Crypt = crypt;
            LongPolling = longPolling;
            Security = security;
            Authentication = authentication;
        }

        public RsaPublicKey RsaKeyTrade(RsaPublicKey clientKey)
        {
            using var encrypter = new RsaEncrypter();
            var serverKey = encrypter.PublicKey;

            var id = Crypt.SaveRsaKeyPair(encrypter.PrivateKey, clientKey.Key,
                                          DateTime.UtcNow + Settings.RsaLifeTime);

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
            var expiration = DateTime.UtcNow.Add(Settings.TokenLifeTime);

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

        public AuthorizationResponse? UpdateAuth(ApiData<AuthUpdateRequest> request)
        {
            if(!Security.IsCryptIdCorrect(request.UserId, request.CryptId))
                return null;
            Crypt.SetDeprecated(request.CryptId);
            var newAuth = GetAuthData(request.UserId);
            return newAuth;
        }

        public AuthorizationResponse? ChangePassword(ApiData<ChangePasswordRequest> request)
        {
            if (!Authentication.IsPasswordMatch(request.UserId, request.Data.CurrentPasswordHash)||
                !Security.IsCryptIdCorrect(request.UserId,request.CryptId))
                return null;
            var result = Authentication.ChangePassword(request.UserId, request.Data.NewPasswordHash);
            if (result)
            {
                Crypt.SetDeprecated(request.CryptId);
                return GetAuthData(request.UserId);
            }
            return null;
        }
    }
}
