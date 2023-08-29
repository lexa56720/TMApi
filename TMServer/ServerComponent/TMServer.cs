﻿using ApiTypes;
using ApiTypes.Communication.Auth;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Chats;
using ApiTypes.Communication.Friends;
using ApiTypes.Communication.Search;
using ApiTypes.Communication.Users;
using TMServer.DataBase;
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

        public TMServer(int authPort, int responsePort)
        {
            AuthServer = new AuthorizationServer(authPort, new AuthEncryptProvider());
            ApiServer = new ResponseServer(responsePort, new ApiEncryptProvider());
            RegisterAuthMethods();
            RegisterApiMethods();
        }

        private void RegisterAuthMethods()
        {
            AuthServer.Register<RsaPublicKey, RsaPublicKey>(AuthHandler.RsaKeyTrade);
            AuthServer.Register<AuthorizationRequest, AuthorizationResponse>(AuthHandler.Auth);
            AuthServer.Register<RegisterRequest, RegisterResponse>(AuthHandler.Register);
        }

        private void RegisterApiMethods()
        {
            ApiServer.RegisterPostHandler<AuthUpdateRequest, AuthorizationResponse>
                (AuthHandler.UpdateAuth, RequestHeaders.UpdateAuth);


            RegisterUserMethods();
            RegisterFriendMethods();
            RegisterChatMethods();
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
        }

        private void RegisterChatMethods()
        {
            ApiServer.RegisterPostHandler<ChatCreationRequest,Chat>
                (ChatsHandler.CreateChat, RequestHeaders.CreateChat);

            ApiServer.RegisterPostHandler<IntContainer, Chat>
                (ChatsHandler.GetChat, RequestHeaders.GetChat);

            ApiServer.RegisterPostHandler<IntArrayContainer, SerializableArray<Chat>>
               (ChatsHandler.GetChats, RequestHeaders.GetChatMany);

            ApiServer.RegisterGetHandler<ChatInvite>
                (ChatsHandler.SendChatInvite, RequestHeaders.SendChatInvite);

            ApiServer.RegisterPostHandler<IntContainer,ChatInvite>
                (ChatsHandler.GetChatInvite, RequestHeaders.GetChatInvite);

            ApiServer.RegisterPostHandler<IntArrayContainer, SerializableArray<ChatInvite>>
                (ChatsHandler.GetChatInvites, RequestHeaders.GetChatInviteMany);

            ApiServer.RegisterGetHandler<RequestResponse>
                (ChatsHandler.ChatInviteResponse, RequestHeaders.ChatInviteRespose);
        }
        public override void Start()
        {
            base.Start();
            AuthServer.Start();
            ApiServer.Start();
        }

        public override void Stop()
        {
            base.Stop();
            AuthServer.Stop();
            ApiServer.Stop();
        }
        public void Dispose()
        {
            AuthServer.Dispose();
        }
    }
}
