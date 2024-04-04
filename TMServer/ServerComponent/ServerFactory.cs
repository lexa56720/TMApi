using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Interaction;
using TMServer.Logger;
using TMServer.RequestHandlers;
using TMServer.ServerComponent.Api;
using TMServer.ServerComponent.Auth;
using TMServer.ServerComponent.Basics;
using TMServer.ServerComponent.Images;
using TMServer.ServerComponent.Info;
using TMServer.ServerComponent.LongPolling;

namespace TMServer.ServerComponent
{
    internal class ServerFactory
    {
        private readonly Authentication Authentication;
        private readonly Crypt Crypt;
        private readonly Messages Messages;
        private readonly Chats Chats;
        private readonly Friends Friends;
        private readonly DataBase.Interaction.Images Images;
        private readonly DataBase.Interaction.LongPolling LongPolling;
        private readonly Security Security;
        private readonly Users Users;

        private readonly AuthHandler AuthHandler;
        private readonly ChatsHandler ChatsHandler;
        private readonly FriendsHandler FriendsHandler;
        private readonly ImageHandler ImageHandler;
        private readonly MessagesHandler MessagesHandler;
        private readonly SearchHandler SearchHandler;
        private readonly UsersHandler UsersHandler;

        private readonly ILogger Logger;

        public ServerFactory(string salt, ILogger logger)
        {
            Authentication = new Authentication(salt);

            Crypt = new Crypt();
            Messages = new Messages();
            Chats = new Chats(Messages);
            Friends = new Friends(Chats);
            Images = new DataBase.Interaction.Images();
            LongPolling = new DataBase.Interaction.LongPolling();

            Security = new Security();
            Users = new Users();
            var Converter = new DbConverter(Images);

            AuthHandler = new AuthHandler(Crypt, LongPolling,Security, Authentication);
            ChatsHandler = new ChatsHandler(Security, Chats, Converter);
            FriendsHandler = new FriendsHandler(Security, Friends, Converter);
            ImageHandler = new ImageHandler(Images, Chats, Users, Security, Converter);
            MessagesHandler = new MessagesHandler(Security, Messages, Converter);
            SearchHandler = new SearchHandler(Users, Converter);
            UsersHandler = new UsersHandler(Users, Chats, Friends, Images, Converter);
            Logger = logger;
        }

        public ApiServer CreateApiServer()
        {
            return new ApiServer(new ApiEncryptProvider(Crypt), Logger)
            {
                Security = Security,
                Users = Users
            };
        }
        public AuthorizationServer CreateAuthServer()
        {
            return new AuthorizationServer(new AuthEncryptProvider(Crypt), Logger)
            {
                Security = Security,
                Users = Users
            };
        }
        public LongPollServer CreateLongPollServer(TimeSpan longPollLifetime)
        {
            return new LongPollServer(longPollLifetime, new ApiEncryptProvider(Crypt), Logger)
            {
                Security = Security,
                Users = Users
            }; ;
        }
        public ImageServer CreateImageServer()
        {
            return new ImageServer(ImageHandler, new ApiEncryptProvider(Crypt), Logger)
            {
                Security = Security,
                Users = Users
            }; ;
        }
        public InfoServer CreateInfoServer(int port, int version)
        {
            return new InfoServer(port, version,  Logger)
            {
                Security = Security,
                Users = Users
            }; ;
        }
        public TMServer CreateMainServer(ApiServer responseServer, AuthorizationServer authServer,
                                         LongPollServer longPollServer, ImageServer imageServer)
        {
            var server = new TMServer(Logger)
            {
                AuthServer = authServer,
                ApiServer = responseServer,
                LongPollServer = longPollServer,
                ImageServer = imageServer,

                AuthHandler = AuthHandler,
                ChatsHandler = ChatsHandler,
                FriendsHandler = FriendsHandler,
                ImageHandler = ImageHandler,
                MessagesHandler = MessagesHandler,
                SearchHandler = SearchHandler,
                UsersHandler = UsersHandler,
            };
            server.Init();
            return server;
        }

    }
}
