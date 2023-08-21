using ApiTypes;
using ApiTypes.Auth;
using CSDTP.Cryptography;
using CSDTP.Cryptography.Algorithms;
using CSDTP.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase;
using TMServer.Utils;

namespace TMServer.RequestHandlers
{
    internal static class AuthHandler
    {
        private static TimeSpan TokenLife = TimeSpan.FromDays(3);

        public static RsaPublicKey RsaKeyTrade(RsaPublicKey clientKey, IPacketInfo info)
        {
            var id = BitConverter.ToUInt32(info.Source.GetAddressBytes());

            using RsaEncrypter encrypter = new RsaEncrypter();
            var serverKey = encrypter.PublicKey;

            Security.SaveRsaKeyPair(id, encrypter.PrivateKey, clientKey.Key);
            return new RsaPublicKey(serverKey);
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


        public static RegisterResponse Register(AuthorizationRequest request)
        {
            var isSuccsessful = Security.IsLoginFree(request.Login);

            if (isSuccsessful)
                isSuccsessful = Security.CreateUser(request.Login, request.Password);

            return new RegisterResponse()
            {
                IsSuccessful = isSuccsessful
            };
        }
    }
}
