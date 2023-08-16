using ApiTypes;
using CSDTP;
using CSDTP.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.ServerComponent.Basics
{
    internal abstract class Server : Startable, IDisposable
    {

        protected Responder Responder { get; set; }


        protected Server(int port)
        {
            Responder = new Responder(TimeSpan.FromSeconds(20), port);
        }
        public void Dispose()
        {
            Responder.Dispose();
        }

        public override void Start()
        {
            base.Start();

            Responder.Start();
        }
        public override void Stop()
        {
            Responder.Stop();
        }


    }
}
