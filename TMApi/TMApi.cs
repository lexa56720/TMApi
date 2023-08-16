using ApiTypes;
using ApiTypes.Auth;
using CSDTP.Cryptography;
using CSDTP.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMApi.ApiRequests.Chats;
using TMApi.ApiRequests.Messages;
using TMApi.ApiRequests.Users;

namespace TMApi
{
    internal class TMApi
    {
        private string AccesToken
        {
            get => accesToken;
            set
            {
                accesToken = value;
                Requester.Token = value;
            }
        }
        private string accesToken;

        private DateTime Expiration { get; set; }

        public int Id { get; private set; }

        public bool IsLoginIn { get; private set; }

        public Users Users { get; private set; }

        public Messages Messages { get; private set; }

        public Chats Chats { get; private set; }

        private RequestSender Requester { get; set; }

        public UserInfo User { get; private set; }

        public async Task<bool> LoginAsync(string login, string password)
        {
            string serverRsaPublicKey;
            var inputDecoder = new RsaEncrypter();

            using (var uncryptRequester = new RequestSender(true))
            {
                serverRsaPublicKey = (await uncryptRequester.PostAsync<RsaPublicKey, RsaPublicKey>(new RsaPublicKey(inputDecoder.PublicKey))).Key;
            };

            var outputCoder = new RsaEncrypter(serverRsaPublicKey);

            AuthorizationResponse? authResult = null;
            using (var rsaRequester = new RequestSender(true, outputCoder, inputDecoder))
            {
                await rsaRequester.PostAsync<AuthorizationResponse, AuthorizationRequest>(new AuthorizationRequest(login, password));
            }

            if (authResult != null && authResult.IsSuccessful)
            {
                AuthComplete(authResult);
                IsLoginIn = true;
            }
            return IsLoginIn;
        }

        public async Task Init()
        {
            Users = new Users(Requester);
            Messages = new Messages(Requester);
            Chats= new Chats(Requester);

            User = await Users.GetUserInfo(Id);
        }
        private void AuthComplete(AuthorizationResponse auth)
        {
            Requester = new RequestSender(false, new AesEncrypter(auth.AesKey))
            {
                Token = auth.AccessToken,
                Id=auth.Id,
            };
            Id = auth.Id;
            AccesToken = auth.AccessToken;
            Expiration = auth.Expiration;
        }
    }
}
