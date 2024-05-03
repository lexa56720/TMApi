using ApiTypes;
using ApiTypes.Communication.Auth;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Chats;
using ApiTypes.Communication.Friends;
using ApiTypes.Communication.Info;
using ApiTypes.Communication.LongPolling;
using ApiTypes.Communication.Medias;
using ApiTypes.Communication.Messages;
using ApiTypes.Communication.Search;
using ApiTypes.Communication.Users;
using CSDTP.Cryptography.Algorithms;
using CSDTP.Cryptography.Providers;
using CSDTP.Requests;
using System.Diagnostics.CodeAnalysis;
using TMServer.DataBase;
using TMServer.DataBase.Interaction;
using TMServer.Logger;
using TMServer.RequestHandlers;
using TMServer.ServerComponent.Api;
using TMServer.ServerComponent.Auth;
using TMServer.ServerComponent.Basics;
using TMServer.ServerComponent.Files;
using TMServer.ServerComponent.LongPolling;

namespace TMServer.ServerComponent
{
    internal class MainServer : Startable, IDisposable
    {
        public required AuthorizationServer AuthServer { get; init; }
        public required ApiServer ApiServer { get; init; }
        public required LongPollServer LongPollServer { get; init; }
        public required FileServer FileServer { get; init; }

        public required AuthHandler AuthHandler { private get; init; }
        public required ChatsHandler ChatsHandler { private get; init; }
        public required FriendsHandler FriendsHandler { private get; init; }
        public required FileHandler FileHandler { private get; init; }
        public required MessagesHandler MessagesHandler { private get; init; }
        public required SearchHandler SearchHandler { private get; init; }
        public required UsersHandler UsersHandler { private get; init; }

        private readonly ILogger Logger;


        public MainServer(ILogger logger)
        {
            Logger = logger;
        }

        public void Init()
        {
            RegisterAuthMethods();
            RegisterApiMethods();
            RegisterLongPollMethods();
        }

        public void Dispose()
        {
            Stop();
            AuthServer.Dispose();
            ApiServer.Dispose();
            LongPollServer.Dispose();
            FileServer.Dispose();

        }
        private void RegisterAuthMethods()
        {
            AuthServer.Register<RsaPublicKey, RsaPublicKey>(AuthHandler.RsaKeyTrade);
            AuthServer.Register<AuthorizationRequest, AuthorizationResponse>(AuthHandler.Login);
            AuthServer.Register<RegistrationRequest, RegistrationResponse>(AuthHandler.Registration);
        }
        private void RegisterApiMethods()
        {
            ApiServer.RegisterRequestHandler<AuthUpdateRequest, AuthorizationResponse>(AuthHandler.UpdateAuth);
            ApiServer.RegisterRequestHandler<ChangePasswordRequest, AuthorizationResponse>(AuthHandler.ChangePassword);

            RegisterMessageMethods();
            RegisterUserMethods();
            RegisterFriendMethods();
            RegisterChatMethods();
        }
        private void RegisterMessageMethods()
        {
            ApiServer.RegisterRequestHandler<LastMessagesRequest, SerializableArray<Message>>(MessagesHandler.GetMessagesByOffset);
            ApiServer.RegisterRequestHandler<MessageSendRequest, Message>(MessagesHandler.NewMessage);
            ApiServer.RegisterRequestHandler<MessageHistoryRequest, SerializableArray<Message>>(MessagesHandler.GetMessagesByLastId);
            ApiServer.RegisterRequestHandler<MessageRequestById, SerializableArray<Message>>(MessagesHandler.GetMessagesById);
            ApiServer.RegisterRequestHandler<MessageRequestByChats, SerializableArray<Message>>(MessagesHandler.GetMessagesByChatIds);

            ApiServer.RegisterDataHandler<MarkAsReaded>(MessagesHandler.MarkAsReaded);

            FileServer.RegisterRequestHandler<MessageWithFilesSendRequest, Message>(FileHandler.MessageWithFiles);
        }
        private void RegisterUserMethods()
        {
            ApiServer.RegisterRequestHandler<UserFullRequest, UserInfo>(UsersHandler.GetUserInfo);
            ApiServer.RegisterRequestHandler<UserRequest, SerializableArray<User>>(UsersHandler.GetUsers);
            ApiServer.RegisterRequestHandler<ChangeNameRequest, User>(UsersHandler.ChangeUserName);
            ApiServer.RegisterRequestHandler<SearchRequest, SerializableArray<User>>(SearchHandler.GetUserByName);

            FileServer.RegisterRequestHandler<ChangeProfileImageRequest, User>(FileHandler.SetProfileImage);
        }
        private void RegisterFriendMethods()
        {
            ApiServer.RegisterRequestHandler<GetFriendRequests, SerializableArray<FriendRequest>>(FriendsHandler.GetFriendRequests);
            ApiServer.RegisterDataHandler<FriendRequest>(FriendsHandler.AddFriendRequest);
            ApiServer.RegisterDataHandler<RequestResponse>(FriendsHandler.FriendRequestResponse);
            ApiServer.RegisterRequestHandler<GetAllFriendRequests, IntArrayContainer>(FriendsHandler.GetAllFriendRequests);
            ApiServer.RegisterDataHandler<FriendRemoveRequest>(FriendsHandler.RemoveFriend);
        }
        private void RegisterChatMethods()
        {
            ApiServer.RegisterRequestHandler<ChatCreationRequest, Chat>(ChatsHandler.CreateChat);
            ApiServer.RegisterRequestHandler<ChatRequest, SerializableArray<Chat>>(ChatsHandler.GetChats);
            ApiServer.RegisterDataHandler<InviteToChatRequest>(ChatsHandler.AddChatInvite);
            ApiServer.RegisterRequestHandler<InviteRequest, SerializableArray<ChatInvite>>(ChatsHandler.GetChatInvites);
            ApiServer.RegisterDataHandler<ResponseToInvite>(ChatsHandler.ChatInviteResponse);
            ApiServer.RegisterRequestHandler<InviteRequestAll, IntArrayContainer>(ChatsHandler.GetAllChatInvites);
            ApiServer.RegisterRequestHandler<ChatRequestAll, IntArrayContainer>(ChatsHandler.GetAllChats);

            ApiServer.RegisterDataHandler<ChatChangeNameRequest>(ChatsHandler.ChangeName);
            ApiServer.RegisterDataHandler<ChatKickRequest>(ChatsHandler.KickUser);

            ApiServer.RegisterDataHandler<ChatLeaveRequest>(ChatsHandler.LeaveChat);

            FileServer.RegisterDataHandler<ChangeCoverRequest>(FileHandler.SetChatCover);
        }

        private void RegisterLongPollMethods()
        {
            LongPollServer.RegisterRequestHandler<LongPollingRequest, Notification>(LongPollServer.LongPollArrived);
        }

        public override async Task Start()
        {
            if (IsRunning)
                return;

            await base.Start();

            await AuthServer.Start();
            await ApiServer.Start();
            await LongPollServer.Start();
            await FileServer.Start();

            Logger.Log("MainServer is ready");
        }
        public override async Task Stop()
        {
            if (!IsRunning)
                return;

            await base.Stop();
            await AuthServer.Stop();
            await ApiServer.Stop();
            await LongPollServer.Stop();
            await FileServer.Stop();

            Logger.Log("MainServer is down");
        }
    }
}
