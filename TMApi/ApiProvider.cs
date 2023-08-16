using ApiTypes.Auth;
using ApiTypes;
using CSDTP.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSDTP;

namespace TMApi
{
    internal class ApiProvider
    {

        public async Task<TMApi?> GetApi(string login, string password)
        {

            var inputDecoder = new RsaEncrypter();

            var rsaData = await GetRsaKeyAndId(inputDecoder);
            var outputCoder = new RsaEncrypter(rsaData.Item1);

            AuthorizationResponse? authResult = null;
            using (var rsaRequester = new RequestSender(true, outputCoder, inputDecoder))
            {
                authResult = (await PostRequest<AuthorizationResponse, AuthorizationRequest>(rsaData.Item2,new AuthorizationRequest(login, password),rsaRequester)).Data;
            }

            if (authResult != null && authResult.IsSuccessful)
            {
                var api = new TMApi(authResult.AccessToken, authResult.Expiration, authResult.Id, authResult.AesKey);
                await api.Init();
                return api;
            }
            return null;
        }

        private async Task<Tuple<string,int>> GetRsaKeyAndId(RsaEncrypter inputDecoder)
        {
            string serverRsaPublicKey;
            int tempId = -1;


            using (var uncryptRequester = new RequestSender(true))
            {
                var request = new RsaPublicKey(inputDecoder.PublicKey);
                var response = await PostRequest<RsaPublicKey, RsaPublicKey>(tempId, request, uncryptRequester);

                serverRsaPublicKey = response.Data.Key;
                tempId = response.Id;
            };
            return new Tuple<string, int>(serverRsaPublicKey, tempId);
        }

        private async Task<UnauthorizedRequest<T>> PostRequest<T,U>(int id,U data,RequestSender sender) where T:ISerializable<T> where U:ISerializable<U> 
        {
            return await sender.PostAsync<UnauthorizedRequest<T>, UnauthorizedRequest<U>>(new UnauthorizedRequest<U>(data,id));
        }
    }
}
