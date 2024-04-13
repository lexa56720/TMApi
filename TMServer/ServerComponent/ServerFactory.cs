﻿using Microsoft.VisualBasic;
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
using TMServer.ServerComponent.Files;
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
        private readonly DataBase.Interaction.Files Files;
        private readonly DataBase.Interaction.LongPolling LongPolling;
        private readonly Security Security;
        private readonly Users Users;

        private readonly AuthHandler AuthHandler;
        private readonly ChatsHandler ChatsHandler;
        private readonly FriendsHandler FriendsHandler;
        private readonly FileHandler FileHandler;
        private readonly MessagesHandler MessagesHandler;
        private readonly SearchHandler SearchHandler;
        private readonly UsersHandler UsersHandler;

        private readonly int MaxFileSizeMB;
        private readonly int MaxFiles;
        private readonly ILogger Logger;

        public ServerFactory(string salt, string filesPath, string imagesPath, int maxFileSizeMB, int maxFiles, ILogger logger)
        {
            Authentication = new Authentication(salt);

            Crypt = new Crypt();
            Messages = new Messages();
            Chats = new Chats(Messages);
            Friends = new Friends(Chats);
            Friends = new Friends(Chats);
            Files = new DataBase.Interaction.Files(filesPath, imagesPath);
            LongPolling = new DataBase.Interaction.LongPolling();

            Security = new Security(maxFileSizeMB, maxFiles);
            Users = new Users();
            var Converter = new DbConverter(Files);

            AuthHandler = new AuthHandler(Crypt, LongPolling, Security, Authentication);
            ChatsHandler = new ChatsHandler(Security, Chats, Converter);
            FriendsHandler = new FriendsHandler(Security, Friends, Converter);
            FileHandler = new FileHandler(Files, Chats, Users, Messages, Security, Converter);
            MessagesHandler = new MessagesHandler(Security, Messages, Converter);
            SearchHandler = new SearchHandler(Users, Converter);
            UsersHandler = new UsersHandler(Users, Chats, Friends, Files, Converter);
            MaxFileSizeMB = maxFileSizeMB;
            MaxFiles = maxFiles;
            Logger = logger;
        }

        public ApiServer CreateApiServer(int port)
        {
            return new ApiServer(port, new ApiEncryptProvider(Crypt), Logger)
            {
                Security = Security,
                Users = Users
            };
        }
        public AuthorizationServer CreateAuthServer(int port)
        {
            return new AuthorizationServer(port, new AuthEncryptProvider(Crypt), Logger)
            {
                Security = Security,
                Users = Users
            };
        }
        public LongPollServer CreateLongPollServer(int port, TimeSpan longPollLifetime)
        {
            return new LongPollServer(port, longPollLifetime, new ApiEncryptProvider(Crypt), Logger)
            {
                Security = Security,
                Users = Users
            }; ;
        }
        public FileServer CreateFileServer(int uploadPort, int downloadPort)
        {
            return new FileServer(uploadPort, downloadPort, FileHandler, new ApiEncryptProvider(Crypt), Logger)
            {
                Security = Security,
                Users = Users
            }; ;
        }
        public InfoServer CreateInfoServer(int port, int version)
        {
            return new InfoServer(port, Logger)
            {
                Version = version,
                MaxAttachments = MaxFiles,
                MaxFileSizeMB = MaxFileSizeMB,
                Security = Security,
                Users = Users
            }; ;
        }
        public TMServer CreateMainServer(ApiServer responseServer, AuthorizationServer authServer,
                                         LongPollServer longPollServer, FileServer fileServer)
        {
            var server = new TMServer(Logger)
            {
                AuthServer = authServer,
                ApiServer = responseServer,
                LongPollServer = longPollServer,
                FileServer = fileServer,

                AuthHandler = AuthHandler,
                ChatsHandler = ChatsHandler,
                FriendsHandler = FriendsHandler,
                FileHandler = FileHandler,
                MessagesHandler = MessagesHandler,
                SearchHandler = SearchHandler,
                UsersHandler = UsersHandler,
            };
            server.Init();
            return server;
        }

    }
}
