
using Microsoft.EntityFrameworkCore;
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

            //var db = new TmdbContext();
            ////db.Database.EnsureDeleted();
            //db.Database.EnsureCreated();

            //db.RsaCrypts.Add(new RsaCrypt() 
            //{ 
            //    Ip = 16,
            //    PrivateServerKey = "FF",
            //    PublicClientKey = "ss", 
            //    CreateDate = DateTime.UtcNow 
            //});
            //db.AesCrypts.Add(new AesCrypt()
            //{
            //    AesKey = "FF",
            //    IV = "FF", 
            //    CryptId = 1 
            //});

            //db.Users.Add(new User
            //{
            //    Login = "fff",
            //    Password = "Fff",
            //    CryptId = 1,
            //    LastRequest= DateTime.UtcNow,
            //    Name = "peter",
            //});
            //db.SaveChanges();
            //db.Dispose();

            //db = new TmdbContext();

            //var a = db.Users.Find(1);
            //Console.WriteLine(a.Crypt.AesKey);
            Servers.TMServer server = new Servers.TMServer(AuthPort, ResponsePort);
            server.Start();
            Console.ReadLine();
        }
    }
}