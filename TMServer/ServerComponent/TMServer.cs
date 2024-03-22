using ApiTypes;
using ApiTypes.Communication.Auth;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Chats;
using ApiTypes.Communication.Friends;
using ApiTypes.Communication.Info;
using ApiTypes.Communication.LongPolling;
using ApiTypes.Communication.Messages;
using ApiTypes.Communication.Search;
using ApiTypes.Communication.Users;
using CSDTP.Cryptography.Algorithms;
using CSDTP.Cryptography.Providers;
using CSDTP.Requests;
using TMServer.DataBase;
using TMServer.Logger;
using TMServer.RequestHandlers;
using TMServer.ServerComponent.ApiResponser;
using TMServer.ServerComponent.Auth;
using TMServer.ServerComponent.Basics;
using TMServer.ServerComponent.LongPolling;

namespace TMServer.Servers
{
    internal class TMServer : Startable, IDisposable
    {
        private readonly AuthorizationServer AuthServer;

        private readonly ResponseServer ApiServer;

        private readonly LongPollServer LongPollServer;

        private readonly ILogger Logger;

        public TMServer(TimeSpan longPollLifetime, int authPort, int responsePort, int longPollPort, ILogger logger)
        {
            AuthServer = new AuthorizationServer(authPort, new AuthEncryptProvider(), logger);
            ApiServer = new ResponseServer(responsePort, new ApiEncryptProvider(), logger);
            LongPollServer = new LongPollServer(longPollLifetime, longPollPort, new ApiEncryptProvider(), logger);

            RegisterAuthMethods();
            RegisterApiMethods();
            RegisterLongPollMethods();

            Logger = logger;
        }
        public void Dispose()
        {
            Stop();
            AuthServer.Dispose();
            ApiServer.Dispose();
            LongPollServer.Dispose();
        }
        private void RegisterAuthMethods()
        {
            AuthServer.Register<RsaPublicKey, RsaPublicKey>(AuthHandler.RsaKeyTrade);
            AuthServer.Register<AuthorizationRequest, AuthorizationResponse>(AuthHandler.Login);
            AuthServer.Register<RegisterRequest, RegisterResponse>(AuthHandler.Register);
            AuthServer.Register<VersionRequest, IntContainer>(e => AuthHandler.GetVersion());
        }

        private void RegisterApiMethods()
        {
            ApiServer.RegisterRequestHandler<AuthUpdateRequest, AuthorizationResponse>(AuthHandler.UpdateAuth);

            RegisterMessageMethods();
            RegisterUserMethods();
            RegisterFriendMethods();
            RegisterChatMethods();
        }
        private void RegisterMessageMethods()
        {
            ApiServer.RegisterRequestHandler<LastMessagesRequest, MessageHistoryResponse>(MessagesHandler.GetMessagesByOffset);
            ApiServer.RegisterRequestHandler<MessageSendRequest, Message>(MessagesHandler.NewMessage);
            ApiServer.RegisterRequestHandler<MessageHistoryRequest, MessageHistoryResponse>(MessagesHandler.GetMessagesByLastId);
            ApiServer.RegisterRequestHandler<MessageRequestById, SerializableArray<Message>>(MessagesHandler.GetMessagesById);
            ApiServer.RegisterRequestHandler<MessageRequestByChats, SerializableArray<Message>>(MessagesHandler.GetMessagesByChatIds);

            ApiServer.RegisterDataHandler<MarkAsReaded>(MessagesHandler.MarkAsReaded);
        }
        private void RegisterUserMethods()
        {
            ApiServer.RegisterRequestHandler<UserFullRequest, UserInfo>(UsersHandler.GetUserInfo);
            ApiServer.RegisterRequestHandler<UserRequest, SerializableArray<User>>(UsersHandler.GetUsers);
            ApiServer.RegisterDataHandler<ChangeNameRequest>(UsersHandler.ChangeUserName);
            ApiServer.RegisterRequestHandler<SearchRequest, SerializableArray<User>>(SearchHandler.GetUserByName);
        }
        private void RegisterFriendMethods()
        {
            ApiServer.RegisterRequestHandler<GetFriendRequests, SerializableArray<FriendRequest>>(FriendsHandler.GetFriendRequests);
            ApiServer.RegisterDataHandler<FriendRequest>(FriendsHandler.AddFriendRequest);
            ApiServer.RegisterDataHandler<RequestResponse>(FriendsHandler.FriendRequestResponse);
            ApiServer.RegisterRequestHandler<GetAllFriendRequests, IntArrayContainer>(FriendsHandler.GetAllFriendRequests);
        }
        private void RegisterChatMethods()
        {
            ApiServer.RegisterRequestHandler<ChatCreationRequest, Chat>(ChatsHandler.CreateChat);
            ApiServer.RegisterRequestHandler<ChatRequest, SerializableArray<Chat>>(ChatsHandler.GetChats);
            ApiServer.RegisterDataHandler<InviteToChatRequest>(ChatsHandler.RegisterChatInvite);
            ApiServer.RegisterRequestHandler<InviteRequest, SerializableArray<ChatInvite>>(ChatsHandler.GetChatInvites);
            ApiServer.RegisterDataHandler<ResponseToInvite>(ChatsHandler.ChatInviteResponse);
            ApiServer.RegisterRequestHandler<InviteRequestAll, IntArrayContainer>(ChatsHandler.GetAllChatInvites);
            ApiServer.RegisterRequestHandler<ChatRequestAll, IntArrayContainer>(ChatsHandler.GetAllChats);

            ApiServer.RegisterDataHandler<ChatLeaveRequest>(ChatsHandler.LeaveChat);
        }

        private void RegisterLongPollMethods()
        {
            LongPollServer.RegisterRequestHandler<LongPollingRequest, Notification>(LongPollServer.LongPollArrived);
        }

        public override void Start()
        {
            if (IsRunning)
                return;

            base.Start();

            AuthServer.Start();
            ApiServer.Start();
            LongPollServer.Start();

            Logger.Log("\nServer is ready\n");
        }
        public override void Stop()
        {
            if (!IsRunning)
                return;

            base.Stop();
            AuthServer.Stop();
            ApiServer.Stop();
            LongPollServer.Stop();

            Logger.Log("\n Server is down");
        }
    }
}
