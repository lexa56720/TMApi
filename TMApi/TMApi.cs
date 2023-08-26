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
using TMApi.ApiRequests.Friends;
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
        private string accesToken=string.Empty;

        private DateTime Expiration { get; set; }

        public int Id { get; private set; }
        private int CryptId { get; }

        private AesEncrypter Encrypter { get; set; }

        public UserInfo User { get; private set; }

        public Users Users { get; private set; }

        public Messages Messages { get; private set; }

        public Chats Chats { get; private set; }

        public Friends Friends { get; private set; }

        private RequestSender Requester { get; set; }

        public TMApi(string token, DateTime tokenTime, int userId, int cryptId, byte[] aesKey)
        {    
            Id = userId;
            CryptId = cryptId;
            AccesToken = token;
            Expiration = tokenTime;
            Encrypter = new AesEncrypter(aesKey);

            SetupRequester(token, userId, cryptId);
        }

        private void SetupRequester(string token, int userId, int cryptId)
        {          
            Requester = new RequestSender(false, Encrypter, Encrypter)
            {
                Token = token,
                UserId = userId,
            };
            IdHolder.Value = cryptId;
        }

        public async Task UpdateAuth()
        {

        }
        public byte[] GetAuthData()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            bw.Write(AccesToken);
            bw.Write(Expiration.ToBinary());
            bw.Write(Encrypter.Key);
            bw.Write(CryptId);
            bw.Write(Id);
            bw.Flush();

            return ms.ToArray();
        }
        public async Task Init()
        {
            Users = new Users(Requester, this);
            Messages = new Messages(Requester, this);
            Chats = new Chats(Requester, this);
            Friends = new Friends(Requester, this);

            User = await Users.GetUserInfo(Id);
        }
    }
}
