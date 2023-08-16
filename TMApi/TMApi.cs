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


        public TMApi(string token, DateTime tokenTime, int id, byte[] aesKey)
        {
            Requester = new RequestSender(false, new AesEncrypter(aesKey))
            {
                Token = token,
                Id = id,
            };
            Id = id;
            AccesToken = token;
            Expiration = tokenTime;
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
