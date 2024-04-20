using ApiTypes;
using ApiTypes.Communication.Auth;
using ApiTypes.Communication.Users;
using ApiTypes.Shared;
using CSDTP.Cryptography.Algorithms;
using TMServer.DataBase.Interaction;

namespace TMServer.RequestHandlers
{
    public class AuthHandler
    {
        private readonly Crypts Crypt;
        private readonly LongPolling LongPolling;
        private readonly Authentications Authentication;
        private readonly Tokens Tokens;
        private readonly Security Security;
        public AuthHandler(Crypts crypt, LongPolling longPolling, Security security, Authentications authentication, Tokens tokens)
        {
            Crypt = crypt;
            LongPolling = longPolling;
            Security = security;
            Authentication = authentication;
            Tokens = tokens;
        }

        public Task<RsaPublicKey?> RsaKeyTrade(RsaPublicKey clientKey)
        {
            using var encrypter = new RsaEncrypter();
            var serverKey = encrypter.PublicKey;

            var rsa = Crypt.AddRsaKeys(encrypter.PrivateKey, clientKey.Key);

            return Task.FromResult<RsaPublicKey?>(new RsaPublicKey(serverKey, rsa.Id));
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
            return await CreateAuth(id);
        }
        private Task<AuthorizationResponse?> CreateAuth(int userId)
        {
            using var aesCrypter = new AesEncrypter();

            var token = Tokens.AddToken(userId);
            var crypt = Crypt.AddAes(userId, aesCrypter.Key);

            return Task.FromResult<AuthorizationResponse?>(new AuthorizationResponse()
            {
                AccessToken = token.AccessToken,
                AesKey = crypt.AesKey,
                Expiration = token.Expiration,
                UserId = userId,
                CryptId = crypt.Id,
                IsSuccessful = true
            });
        }

        public async Task<RegisterResponse?> Register(RegisterRequest request)
        {
            var isSuccsessful = DataConstraints.IsLoginLegal(request.Login)
                && await Authentication.IsLoginAvailable(request.Login);

            if (isSuccsessful)
            {
                using var aes = new AesEncrypter();
                var user = await Authentication.CreateUser(request.Username, request.Login, request.Password);
                Crypt.AddAes(user.Id, aes.Key);
            }

            return new RegisterResponse(isSuccsessful);
        }

        public async Task<AuthorizationResponse?> UpdateAuth(ApiData<AuthUpdateRequest> request)
        {
            if (!await Security.IsCryptIdCorrect(request.UserId, request.CryptId))
                return null;
            Crypt.SetDeprecated(request.CryptId);
            var newAuth = await CreateAuth(request.UserId);
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
                Crypt.SetDeprecated(request.CryptId);
                return await CreateAuth(request.UserId);
            }
            return null;
        }
    }
}
