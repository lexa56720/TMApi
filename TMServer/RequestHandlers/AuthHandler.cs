﻿using ApiTypes;
using ApiTypes.Auth;
using ApiTypes.Shared;
using CSDTP.Cryptography;
using CSDTP.Cryptography.Algorithms;
using CSDTP.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase;


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
            var id = Security.GetUserId(request.Login, request.Password);
            if (id < 0)
                return new AuthorizationResponse()
                {
                    IsSuccessful = false
                };

            return GetAuthData(id);
        }

        //Надо удалять старые записи аунтефикаций и рса ключи
        //При обнове обновлять аес ключ и айди в бд после отвравки нового ключа
        private static AuthorizationResponse GetAuthData(int userId)
        {
            var token = HashGenerator.GetRandomString();
            var aesCrypter = new AesEncrypter();
            var expiration = DateTime.UtcNow.Add(TokenLife);

            var cryptId = Security.SaveAuth(userId, aesCrypter.Key, aesCrypter.IV, token, expiration);

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
            var isSuccsessful = DataConstraints.IsLoginLegal(request.Login) && Security.IsLoginAvailable(request.Login);

            if (isSuccsessful)
                isSuccsessful = Security.CreateUser(request.Login, request.Password);

            return new RegisterResponse()
            {
                IsSuccessful = isSuccsessful
            };
        }

        public static AuthorizationResponse UpdateAuth(ApiData<AuthUpdateRequest> request)
        {
            return GetAuthData(request.UserId);
        }
    }
}
