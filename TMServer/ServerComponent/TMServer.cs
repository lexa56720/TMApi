using ApiTypes;
using ApiTypes.Communication.Auth;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Chats;
using ApiTypes.Communication.Friends;
using ApiTypes.Communication.Info;
using ApiTypes.Communication.Messages;
using ApiTypes.Communication.Search;
using ApiTypes.Communication.Users;
using TMServer.DataBase;
using TMServer.Logger;
using TMServer.RequestHandlers;
using TMServer.ServerComponent.ApiResponser;
using TMServer.ServerComponent.Auth;
using TMServer.ServerComponent.Basics;

namespace TMServer.Servers
{
    internal class TMServer : Startable, IDisposable
    {
        private AuthorizationServer AuthServer { get; set; }

        private ResponseServer ApiServer { get; set; }
        private ILogger Logger { get; }

        public TMServer(int authPort, int responsePort, ILogger logger)
        {
            AuthServer = new AuthorizationServer(authPort, new AuthEncryptProvider(), logger);
            ApiServer = new ResponseServer(responsePort, new ApiEncryptProvider(), logger);
            RegisterAuthMethods();
            RegisterApiMethods();
            Logger = logger;
        }
        public void Dispose()
        {
            AuthServer.Dispose();
            ApiServer.Dispose();
        }
        private void RegisterAuthMethods()
        {
            AuthServer.Register<RsaPublicKey, RsaPublicKey>(AuthHandler.RsaKeyTrade);
            AuthServer.Register<AuthorizationRequest, AuthorizationResponse>(AuthHandler.Auth);
            AuthServer.Register<RegisterRequest, RegisterResponse>(AuthHandler.Register);
            AuthServer.Register<VersionRequest, IntContainer>(e => AuthHandler.GetVersion());
        }

        private void RegisterApiMethods()
        {
            ApiServer.RegisterPostHandler<AuthUpdateRequest, AuthorizationResponse>
                (AuthHandler.UpdateAuth, RequestHeaders.UpdateAuth);

            RegisterMessageMethods();
            RegisterUserMethods();
            RegisterFriendMethods();
            RegisterChatMethods();
        }
        private void RegisterMessageMethods()
        {
            ApiServer.RegisterPostHandler<MessageHistoryRequest, MessageHistoryResponse>
                (MessagesHandler.GetMessages, RequestHeaders.GetLastMessages);

            ApiServer.RegisterPostHandler<MessageSendRequest, Message>
              (MessagesHandler.NewMessage, RequestHeaders.SendMessage);
        }
        private void RegisterUserMethods()
        {
            ApiServer.RegisterPostHandler<IntContainer, UserInfo>
                (UsersHandler.GetUserInfo, RequestHeaders.GetUserInfo);

            ApiServer.RegisterPostHandler<IntContainer, User>
                (UsersHandler.GetUser, RequestHeaders.GetUser);

            ApiServer.RegisterPostHandler<IntArrayContainer, SerializableArray<User>>
                (UsersHandler.GetUsers, RequestHeaders.GetUserMany);

            ApiServer.RegisterGetHandler<ChangeNameRequest>
                (UsersHandler.ChangeUserName, RequestHeaders.ChangeName);

            ApiServer.RegisterPostHandler<SearchRequest, SerializableArray<User>>
                (SearchHandler.GetUserByName, RequestHeaders.SearchByName);

        }
        private void RegisterFriendMethods()
        {
            ApiServer.RegisterPostHandler<IntContainer, FriendRequest>
                (FriendsHandler.GetFriendRequest, RequestHeaders.GetFriendRequest);

            ApiServer.RegisterPostHandler<IntArrayContainer, SerializableArray<FriendRequest>>
                (FriendsHandler.GetFriendRequests, RequestHeaders.GetFriendRequestMany);

            ApiServer.RegisterGetHandler<FriendRequest>
                (FriendsHandler.AddFriendRequest, RequestHeaders.SendFriendRequest);

            ApiServer.RegisterGetHandler<RequestResponse>
                (FriendsHandler.FriendRequestResponse, RequestHeaders.ResponseFriendRequest);

            ApiServer.RegisterPostHandler<IntContainer, IntArrayContainer>
                (FriendsHandler.GetAllFriendRequests, RequestHeaders.GetAllFriendRequestForUser);
        }
        private void RegisterChatMethods()
        {
            ApiServer.RegisterPostHandler<ChatCreationRequest, Chat>
                (ChatsHandler.CreateChat, RequestHeaders.CreateChat);

            ApiServer.RegisterPostHandler<IntContainer, Chat>
                (ChatsHandler.GetChat, RequestHeaders.GetChat);

            ApiServer.RegisterPostHandler<IntArrayContainer, SerializableArray<Chat>>
               (ChatsHandler.GetChats, RequestHeaders.GetChatMany);

            ApiServer.RegisterGetHandler<ChatInvite>
                (ChatsHandler.SendChatInvite, RequestHeaders.SendChatInvite);

            ApiServer.RegisterPostHandler<IntContainer, ChatInvite>
                (ChatsHandler.GetChatInvite, RequestHeaders.GetChatInvite);

            ApiServer.RegisterPostHandler<IntArrayContainer, SerializableArray<ChatInvite>>
                (ChatsHandler.GetChatInvites, RequestHeaders.GetChatInviteMany);

            ApiServer.RegisterGetHandler<RequestResponse>
                (ChatsHandler.ChatInviteResponse, RequestHeaders.ChatInviteRespose);

            ApiServer.RegisterPostHandler<IntContainer, IntArrayContainer>
                (ChatsHandler.GetAllChatInvites, RequestHeaders.GetAllChatInvitesForUser);
        }

        public override void Start()
        {
            base.Start();
            AuthServer.Start();
            ApiServer.Start();
            Logger.Log("server started");
        }

        public override void Stop()
        {
            base.Stop();
            AuthServer.Stop();
            ApiServer.Stop();
            Logger.Log("server stoped");
        }

    }
}
