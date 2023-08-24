using ApiTypes;
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
            if (id > 0)
            {
                var token = HashGenerator.GetRandomString();
                var aesCrypter = new AesEncrypter();
                var expiration = DateTime.UtcNow.Add(TokenLife);

                var cryptId = Security.SaveAuth(id, aesCrypter.Key, aesCrypter.IV, token, expiration);

                return new AuthorizationResponse()
                {
                    AccessToken = token,
                    AesKey = aesCrypter.Key,
                    Expiration = expiration,
                    UserId = id,
                    CryptId = cryptId,
                    IsSuccessful = true
                };
            }
            return new AuthorizationResponse()
            {
                IsSuccessful = false
            };
        }


        public static RegisterResponse Register(RegisterRequest request)
        {
            var isSuccsessful = Security.IsLoginAvailable(request.Login);

            if (isSuccsessful)
                isSuccsessful = Security.CreateUser(request.Login, request.Password);

            return new RegisterResponse()
            {
                IsSuccessful = isSuccsessful
            };
        }
    }
}
