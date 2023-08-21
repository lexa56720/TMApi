using ApiTypes.Auth;
using ApiTypes;
using CSDTP.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSDTP;
using CSDTP.Cryptography.Algorithms;
using CSDTP.Protocols;
using System.Reflection;

namespace TMApi
{
    internal class ApiProvider
    {

        public async Task<TMApi?> GetApi(string login, string password)
        {

            var inputDecoder = new RsaEncrypter();

            var rsaKey = await GetRsaKeyAndId(inputDecoder);
            var outputCoder = new RsaEncrypter(rsaKey);

            AuthorizationResponse? authResult = null;
            using (var rsaRequester = new RequestSender(true, outputCoder, inputDecoder))
            {
                authResult = await rsaRequester.PostAsync<AuthorizationResponse, AuthorizationRequest>(new AuthorizationRequest(login, password));
            }

            if (authResult != null && authResult.IsSuccessful)
            {
                var api = new TMApi(authResult.AccessToken, authResult.Expiration, authResult.UserId, authResult.CryptId, authResult.AesKey);
                await api.Init();
                return api;
            }
            return null;
        }

        private async Task<string> GetRsaKeyAndId(RsaEncrypter inputDecoder)
        {
            string serverRsaPublicKey;


            using (var uncryptRequester = new RequestSender(true))
            {
                var request = new RsaPublicKey(inputDecoder.PublicKey);
                var response = await uncryptRequester.PostAsync<RsaPublicKey, RsaPublicKey>(request);

                serverRsaPublicKey = response.Key;
            };
            return serverRsaPublicKey;
        }

    }
}
