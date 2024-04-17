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

        public async Task<RsaPublicKey?> RsaKeyTrade(RsaPublicKey clientKey)
        {
            using var encrypter = new RsaEncrypter();
            var serverKey = encrypter.PublicKey;

            var id = await Crypt.SaveRsaKeyPair(encrypter.PrivateKey, clientKey.Key,
                                          DateTime.UtcNow + Settings.RsaLifeTime);

            return new RsaPublicKey(serverKey, id);
        }

        public async Task<AuthorizationResponse?> Login(AuthorizationRequest request)
        {
            var id = await Authentication.GetUserId(request.Login, request.Password);
            if (id < 0)
                return new AuthorizationResponse()
                {
                    IsSuccessful = false
                };
            await LongPolling.ClearAllUpdates(id);
            return await GetAuthData(id);
        }
        private async Task<AuthorizationResponse?> GetAuthData(int userId)
        {
            var token = HashGenerator.GetRandomString();
            var aesCrypter = new AesEncrypter();
            var expiration = DateTime.UtcNow.Add(Settings.TokenLifeTime);

            var cryptId = await Authentication.SaveAuth(userId, aesCrypter.Key, token, expiration);

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

        public async Task<RegisterResponse?> Register(RegisterRequest request)
        {
            var isSuccsessful = DataConstraints.IsLoginLegal(request.Login)
                && await Authentication.IsLoginAvailable(request.Login);

            if (isSuccsessful)
            {
                using var aes = new AesEncrypter();
                await Authentication.CreateUser(request.Username, request.Login, request.Password, aes.Key);
            }

            return new RegisterResponse(isSuccsessful);
        }

        public async Task<AuthorizationResponse?> UpdateAuth(ApiData<AuthUpdateRequest> request)
        {
            if (!await Security.IsCryptIdCorrect(request.UserId, request.CryptId))
                return null;
            await Crypt.SetDeprecated(request.CryptId);
            var newAuth = await GetAuthData(request.UserId);
            return newAuth;
        }

        public async Task<AuthorizationResponse?> ChangePassword(ApiData<ChangePasswordRequest> request)
        {
            if (!await Authentication.IsPasswordMatch(request.UserId, request.Data.CurrentPasswordHash) ||
                !await Security.IsCryptIdCorrect(request.UserId, request.CryptId))
                return null;
            var result = await Authentication.ChangePassword(request.UserId, request.Data.NewPasswordHash);
            if (result)
            {
                await Crypt.SetDeprecated(request.CryptId);
                return await GetAuthData(request.UserId);
            }
            return null;
        }
    }
}
