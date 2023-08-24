using ApiTypes.Auth;
using ApiTypes.BaseTypes;
using ApiTypes.Packets;
using ApiTypes.Users;
using CSDTP.Cryptography;
using CSDTP.Cryptography.Algorithms;
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
    public class TMApi
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


        public TMApi(string token, DateTime tokenTime, int userId, int cryptId, byte[] aesKey)
        {
            SetupRequester(token, userId, cryptId, aesKey);

            Id = userId;
            AccesToken = token;
            Expiration = tokenTime;
        }

        private void SetupRequester(string token, int userId, int cryptId, byte[] aesKey)
        {
            var crypter = new AesEncrypter(aesKey);
            Requester = new RequestSender(false, crypter, crypter)
            {
                Token = token,
                UserId = userId,
            };
            IdHolder.Value = cryptId;
        }

        public async Task Init()
        {
            Users = new Users(Requester);
            Messages = new Messages(Requester);
            Chats = new Chats(Requester);
            User = await Users.GetUserInfo(Id);
        }

    }
}
