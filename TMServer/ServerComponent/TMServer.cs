﻿using ApiTypes;
using ApiTypes.Auth;
using ApiTypes.BaseTypes;
using ApiTypes.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.RequestHandlers;
using TMServer.ServerComponent.ApiResponser;
using TMServer.ServerComponent.Auth;
using TMServer.ServerComponent.Basics;

namespace TMServer.Servers
{
    internal class TMServer : Startable, IDisposable
    {
        private AuthorizationServer AuthServer { get; set; }

        private ResponseServer ResponseServer { get; set; }

        public TMServer(int authPort, int responsePort)
        {
            AuthServer = new AuthorizationServer(authPort, new AuthEncryptProvider());
            ResponseServer = new ResponseServer(responsePort, new ApiEncryptProvider());
            RegisterAuthMethods();
            RegisterResponseMethods();
        }

        public void RegisterAuthMethods()
        {
            AuthServer.Register<RsaPublicKey, RsaPublicKey>(AuthHandler.RsaKeyTrade);
            AuthServer.Register<AuthorizationRequest, AuthorizationResponse>(AuthHandler.Auth);
            AuthServer.Register<RegisterRequest, RegisterResponse>(AuthHandler.Register);
        }

        public void RegisterResponseMethods()
        {
            ResponseServer.RegisterPostHandler<IntContainer, UserInfo>(UsersHandler.GetUserInfo, RequestHeaders.GetUserInfo);
        }
        public override void Start()
        {
            base.Start();
            AuthServer.Start();
            ResponseServer.Start();
        }

        public override void Stop()
        {
            base.Stop();
            AuthServer.Stop();
            ResponseServer.Stop();
        }
        public void Dispose()
        {
            AuthServer.Dispose();
        }
    }
}
