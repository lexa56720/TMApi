using ApiTypes;
using ApiTypes.Packets;
using CSDTP;
using CSDTP.Cryptography.Providers;
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


        protected Server(int port,IEncryptProvider encryptProvider, IEncryptProvider decryptProvider)
        {
            Responder = new Responder(TimeSpan.FromSeconds(20), port, encryptProvider, decryptProvider);
            Responder.SetPacketType(typeof(TMPacket<>));
        }

        protected Server(int port, IEncryptProvider twoWayProvider)
        {
            Responder = new Responder(TimeSpan.FromSeconds(20), port, twoWayProvider, twoWayProvider);
            Responder.SetPacketType(typeof(TMPacket<>));
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
