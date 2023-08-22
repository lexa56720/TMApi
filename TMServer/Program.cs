
using TMServer.DataBase;
using TMServer.DataBase.Tables;

namespace TMServer
{
    internal class Program
    {
        static int AuthPort = 6665;
        static int ResponsePort = 6666;


        static void Main(string[] args)
        {

            using var db = new TmdbContext();
            db.RsaCrypts.Add(new RsaCrypt() {Ip=12,  PrivateServerKey="FF",PublicClientKey="ss"});
            db.SaveChanges();
           // Servers.TMServer server = new Servers.TMServer(AuthPort, ResponsePort);
           // server.Start();
        }
    }
}