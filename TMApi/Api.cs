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
  

        public event EventHandler<Notification>? UpdateArrived;
        private AesEncrypter Encrypter { get; set; }

        public UserInfo UserInfo { get; private set; }
        public Users Users { get; private set; }
        public Messages Messages { get; private set; }
        public Auth Auth { get; set; }
        public Chats Chats { get; private set; }
        public Friends Friends { get; private set; }
        private RequestSender Requester { get; set; }
        private LongPolling LongPolling { get; set; }

        internal Api(string token, DateTime tokenTime, int userId, int cryptId, byte[] aesKey,
                     IPAddress server, int authPort, int apiPort, int longPollPort)
        {
            Id = userId;
            AccesToken = token;
            Expiration = tokenTime;
            Encrypter = new AesEncrypter(aesKey);
       
            Preferences.CtyptId = cryptId;

            Requester = new RequestSender(RequestKind.Request, Encrypter, Encrypter)
            {
                Token = token,
                UserId = userId,
                Server = server,
                AuthPort = authPort,
                ApiPort = apiPort,
                LongPollPort = longPollPort,
            };
        }
        internal async Task<bool> Init(TimeSpan longPollPeriod)
        {
            Users = new Users(Requester, this);
            Messages = new Messages(Requester, this);
            Chats = new Chats(Requester, this);
            Friends = new Friends(Requester, this);
            Auth = new Auth(Requester, this);
            LongPolling = new LongPolling(longPollPeriod,Requester, this);
            LongPolling.StateUpdated += OnUpdateArrived;

            UserInfo = await Users.GetUserInfo(Id);

            return UserInfo != null;
        }


        public void StartLongPolling()
        {
            LongPolling.Start();
        }
        public void StopLongPolling()
        {
            LongPolling.Stop();
        }
        public void Dispose()
        {
            Encrypter.Dispose();

            Users.Dispose();
            Messages.Dispose();
            Chats.Dispose();
            Friends.Dispose();
            Auth.Dispose();

            LongPolling.StateUpdated -= OnUpdateArrived;
            LongPolling.Dispose();
        }

        private void OnUpdateArrived(object? o, Notification e)
        {
            UpdateArrived?.Invoke(this, e);
        }

        public byte[] SerializeAuthData()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            bw.Write(AccesToken);
            bw.Write(Expiration.ToBinary());
            bw.Write(Encrypter.Key);
            bw.Write(Preferences.CtyptId);
            bw.Write(Id);
            bw.Flush();

            return ms.ToArray();
        }

        public void UpdateApiData(AuthorizationResponse response)
        {
            Preferences.CtyptId = response.CryptId;

            Encrypter.Key = response.AesKey;
            Requester.Token = response.AccessToken;
            AccesToken = response.AccessToken;
            Id = Requester.UserId = response.UserId;
            Expiration = response.Expiration;
        }
    }
}
