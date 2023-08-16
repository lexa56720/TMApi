using ApiTypes;
using ApiTypes.Auth;
using CSDTP.Cryptography;
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

        public static UnauthorizedRequest<RsaPublicKey> RsaKeyTrade(UnauthorizedRequest<RsaPublicKey> clientKey)
        {
            using RsaEncrypter encrypter = new RsaEncrypter();
            var serverKey = encrypter.PublicKey;
            var id = Security.SaveRsaKeyPair(encrypter.PrivateKey, clientKey.Data.Key);
            return new UnauthorizedRequest<RsaPublicKey>(new RsaPublicKey(serverKey), id);
        }


        public static UnauthorizedRequest<AuthorizationResponse> Auth(UnauthorizedRequest<AuthorizationRequest> request)
        {
            Security.GetUserId(r)
        }
    }
}
