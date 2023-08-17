﻿using ApiTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.RequestHandlers;
using TMServer.ServerComponent.Auth;
using TMServer.ServerComponent.Basics;

namespace TMServer.Servers
{
    internal class TMServer : Startable, IDisposable
    {
        private AuthorizationServer AuthServer { get; set; }
        public TMServer(int authPort, int responsePort)
        {
            AuthServer = new AuthorizationServer(authPort);
        }

        public void RegisterAuthMethods()
        {
            AuthServer.Register< RsaPublicKey, RsaPublicKey>(AuthHandler.RsaKeyTrade);
        }

        public override void Start()
        {
            base.Start();
            AuthServer.Start();
        }

        public override void Stop()
        {
            base.Stop();
            AuthServer.Stop();
        }
        public void Dispose()
        {
            AuthServer.Dispose();
        }
    }
}
