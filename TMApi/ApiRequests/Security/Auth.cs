using ApiTypes;
using ApiTypes.Communication.Auth;
using ApiTypes.Shared;
using CSDTP.Cryptography.Algorithms;
using TMApi.API;

namespace TMApi.ApiRequests.Security
{
    public class Auth : BaseRequester
    {
        internal Auth(RequestSender requester, Api api) : base(requester, api)
        {
        }

        public async Task<AuthorizationResponse?> UpdateAuth()
        {
            return await Requester.ApiRequestAsync<AuthorizationResponse, AuthUpdateRequest>(new AuthUpdateRequest());
        }

        internal static async Task<(string publicKey, int id)> GetRsaKey(RequestSender uncryptRequester, RsaEncrypter inputDecoder)
        {
            var request = new RsaPublicKey(inputDecoder.PublicKey);
            var response = await uncryptRequester.RequestAsync<RsaPublicKey, RsaPublicKey>(request)
                ?? throw new Exception("no response");

            string serverRsaPublicKey = response.Key;
            return (serverRsaPublicKey, response.Id);
        }


        internal static async Task<AuthorizationResponse?> Login(string login, string password, RequestSender rsaRequester)
        {
            password = HashGenerator.GetPasswordHash(password, login);

            var authResult = await rsaRequester.RequestAsync<AuthorizationResponse, AuthorizationRequest>
                                                              (new AuthorizationRequest(login, password));
            rsaRequester.Dispose();

            return authResult;
        }
    }
}
