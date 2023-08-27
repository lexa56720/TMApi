﻿using ApiTypes.Communication.Auth;
using ApiTypes.Communication.Packets;
using ApiTypes.Shared;
using CSDTP.Cryptography.Algorithms;

namespace TMApi
{
    public class ApiProvider
    {
        public async Task<TMApi?> GetApiRegister(string login, string password)
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

        public async Task<TMApi?> GetApiLogin(string login, string password)
        {
            var coderDecoder = await GetCoderDecoder();
            using var inputDecoder = coderDecoder.Item1;
            using var outputEncoder = coderDecoder.Item2;

            return await Login(login, password, inputDecoder, outputEncoder);
        }

        private async Task<TMApi?> Login(string login, string password, RsaEncrypter inputDecoder, RsaEncrypter outputEncoder)
        {
            password = GetPasswordHash(password);
            AuthorizationResponse? authResult = null;
            using (var rsaRequester = new RequestSender(true, outputEncoder, inputDecoder))
            {
                authResult = await rsaRequester.PostAsync<AuthorizationResponse, AuthorizationRequest>(new AuthorizationRequest(login, password));
            }

            if (authResult != null && authResult.IsSuccessful)
                return await GetApi(authResult.AccessToken, authResult.Expiration,
                                    authResult.UserId, authResult.CryptId, authResult.AesKey);
            return null;
        }

        public async Task<TMApi?> Load(byte[] authData)
        {
            using var ms = new MemoryStream(authData);
            using var br = new BinaryReader(ms);

            var token = br.ReadString();
            var expiration = DateTime.FromBinary(br.ReadInt64());
            var key = br.ReadBytes(32);
            var cryptId = br.ReadInt32();
            var id = br.ReadInt32();

            if (expiration < DateTime.UtcNow.AddHours(1))
                return null;


            var api = await GetApi(token, expiration, id, cryptId, key);
            var newData = await api.Auth.UpdateAuth();

            if (newData != null && newData.IsSuccessful)
                api.UpdateData(newData);
            return api;
        }

        private async Task<TMApi?> GetApi(string token, DateTime expiration, int userId, int cryptId, byte[] aesKey)
        {
            var api = new TMApi(token, expiration, userId, cryptId, aesKey);
            await api.Init();
            return api;
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
