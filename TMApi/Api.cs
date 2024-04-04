using ApiTypes.Communication.Auth;
using ApiTypes.Communication.LongPolling;
using ApiTypes.Communication.Packets;
using ApiTypes.Communication.Users;
using CSDTP.Cryptography.Algorithms;
using System.Net;
using TMApi.ApiRequests;
using TMApi.ApiRequests.Chats;
using TMApi.ApiRequests.Friends;
using TMApi.ApiRequests.Messages;
using TMApi.ApiRequests.Security;
using TMApi.ApiRequests.Users;

namespace TMApi
{
    public class Api : IDisposable
    {
        public int Id { get; private set; }

        private string AccesToken
        {
            get => accesToken;
            set
            {
                accesToken = value;
            }
        }
        private string accesToken = string.Empty;
        private DateTime Expiration { get; set; }

        private ApiEncryptProvider Encrypter { get; set; }

        public UserInfo UserInfo { get; private set; }
        public Users Users { get; private set; }
        public Messages Messages { get; private set; }
        public Auth Auth { get; set; }
        public Chats Chats { get; private set; }
        public Friends Friends { get; private set; }
        public LongPolling LongPolling { get; private set; }
        private RequestSender Requester { get; set; }

        private AuthUpdater AuthUpdater { get; set; }

        internal Api(string token, DateTime tokenTime, int userId, int cryptId, byte[] aesKey,
                     IPAddress server, int authPort, int apiPort, int longPollPort, int imageUploadPort)
        {
            Id = userId;
            AccesToken = token;
            Expiration = tokenTime;
            Encrypter = new ApiEncryptProvider(cryptId,aesKey);
            AuthUpdater = new(this);
            Requester = new RequestSender(server, authPort, apiPort, longPollPort, imageUploadPort, 
                                         RequestKind.Request, Encrypter)
            {
                Token = token,
                UserId = userId,
            };
        }
        internal async Task<bool> Init(TimeSpan longPollPeriod)
        {
            Users = new Users(Requester, this);
            Messages = new Messages(Requester, this);
            Chats = new Chats(Requester, this);
            Friends = new Friends(Requester, this);
            Auth = new Auth(Requester, this);
            LongPolling = new LongPolling(longPollPeriod, Requester, this);

            UserInfo = await Users.GetUserInfo();
            if (UserInfo != null)
                await AuthUpdater.StartUpdate(Expiration - DateTime.UtcNow - TimeSpan.FromHours(2));
            return UserInfo != null;
        }

        public void Dispose()
        {
            Encrypter.Dispose();

            Users.Dispose();
            Messages.Dispose();
            Chats.Dispose();
            Friends.Dispose();
            Auth.Dispose();

            LongPolling.Dispose();
        }

        public byte[] SerializeAuthData()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            (int cryptId, AesEncrypter encrypter) = Encrypter.GetLast();

            bw.Write(AccesToken);
            bw.Write(Expiration.ToBinary());
            bw.Write(encrypter.Key);
            bw.Write(cryptId);
            bw.Write(Id);
            bw.Flush();

            return ms.ToArray();
        }
        public async void UpdateApiData(AuthorizationResponse response)
        {
            Encrypter.Add(response.CryptId, response.AesKey);
            Requester.Token = response.AccessToken;
            AccesToken = response.AccessToken;
            Id = Requester.UserId = response.UserId;
            Expiration = response.Expiration;
            await AuthUpdater.StartUpdate(Expiration - DateTime.UtcNow - TimeSpan.FromHours(2));
        }

    }
}
