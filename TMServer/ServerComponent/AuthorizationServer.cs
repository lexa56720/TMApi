using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.ServerComponent.Basics;

namespace TMServer.Servers
{
    internal class AuthorizationServer : Server
    {
        public AuthorizationServer(int port) : base(port)
        {
        }

        public void Register<T, U>(Func<T, U> func) where T : ISerializable<T> where U : ISerializable<U>
        {
            Responder.RegisterPostHandler(func);
        }

    }
}
