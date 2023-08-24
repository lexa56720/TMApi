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
using ApiTypes.Packets;
using ApiTypes.Shared;

namespace TMApi
{
    public class ApiProvider
    {
        public async Task<TMApi?> Register(string login, string password)
        {
            var coderDecoder = await GetCoderDecoder();
            using var inputDecoder = coderDecoder.Item1;
            using var outputEncoder = coderDecoder.Item2;

            RegisterResponse? registerResult = null;
            using var rsaRequester = new RequestSender(true, outputEncoder, inputDecoder);

            registerResult = await rsaRequester.PostAsync<RegisterResponse, RegisterRequest>(new RegisterRequest(login, GetPasswordHash(password)));


            if (registerResult.IsSuccessful)
                return await Login(login, password, inputDecoder, outputEncoder);

            return null;
        }

        public async Task<TMApi?> GetApi(string login, string password)
        {
            var coderDecoder = await GetCoderDecoder();
            using var inputDecoder = coderDecoder.Item1;
            using var outputEncoder = coderDecoder.Item2;

            return await Login(login, password, inputDecoder, outputEncoder);
        }

        public async Task<TMApi?> Login(string login, string password, RsaEncrypter inputDecoder, RsaEncrypter outputEncoder)
        {
            password = GetPasswordHash(password);
            AuthorizationResponse? authResult = null;
            using (var rsaRequester = new RequestSender(true, outputEncoder, inputDecoder))
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

        private async Task<(RsaEncrypter, RsaEncrypter)> GetCoderDecoder()
        {
            var inputDecoder = new RsaEncrypter();

            var rsaKey = await GetRsaKey(inputDecoder);
            var outputEncoder = new RsaEncrypter(rsaKey.Item1);

            IdHolder.Value = rsaKey.Item2;

            return (inputDecoder, outputEncoder);
        }
        private async Task<(string, int)> GetRsaKey(RsaEncrypter inputDecoder)
        {
            using var uncryptRequester = new RequestSender(true);
            var request = new RsaPublicKey(inputDecoder.PublicKey);
            var response = await uncryptRequester.PostAsync<RsaPublicKey, RsaPublicKey>(request);

            string serverRsaPublicKey = response.Key;
            return (serverRsaPublicKey, response.Id);
        }

        private string GetPasswordHash(string password)
        {
            return HashGenerator.GenerateHash(password);
        }

    }
}
