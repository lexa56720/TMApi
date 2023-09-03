﻿using ApiTypes;
using ApiTypes.Communication.Auth;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Shared;
using CSDTP.Cryptography.Algorithms;
using CSDTP.Packets;
using TMServer.DataBase;
using TMServer.DataBase.Interaction;

namespace TMServer.RequestHandlers
{
    internal static class AuthHandler
    {
        private static TimeSpan TokenLife = TimeSpan.FromDays(3);

        public static RsaPublicKey RsaKeyTrade(RsaPublicKey clientKey, IPacketInfo info)
        {
            using RsaEncrypter encrypter = new RsaEncrypter();
            var serverKey = encrypter.PublicKey;

            var id = Security.SaveRsaKeyPair(encrypter.PrivateKey, clientKey.Key);
            return new RsaPublicKey(serverKey, id);
        }

        public static AuthorizationResponse Auth(AuthorizationRequest request)
        {
            var id = Authentication.GetUserId(request.Login, request.Password);
            if (id < 0)
                return new AuthorizationResponse()
                {
                    IsSuccessful = false
                };

            return GetAuthData(id);
        }
        private static AuthorizationResponse GetAuthData(int userId)
        {
            var token = HashGenerator.GetRandomString();
            var aesCrypter = new AesEncrypter();
            var expiration = DateTime.UtcNow.Add(TokenLife);

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

        public static RegisterResponse Register(RegisterRequest request)
        {
            var isSuccsessful = DataConstraints.IsLoginLegal(request.Login) && Authentication.IsLoginAvailable(request.Login);

            if (isSuccsessful)
            {
                using var aes = new AesEncrypter();
                Authentication.CreateUser(request.Username,request.Login, request.Password, aes.Key);
            }

            return new RegisterResponse()
            {
                IsSuccessful = isSuccsessful
            };
        }

        public static IntContainer GetVersion()
        {
            return new IntContainer(GlobalSettings.Version);
        }

        public static AuthorizationResponse UpdateAuth(ApiData<AuthUpdateRequest> request)
        {
            Security.SetDeprecated(request.CryptId);
            var newAuth = GetAuthData(request.UserId);
            return newAuth;
        }
    }
}
