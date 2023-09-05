using ApiTypes.Shared;
using System.Net;
using TMApi;

namespace TMClient
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var config = new Configurator("config.cfg", true);

            var ip = IPAddress.Parse(config.ConfigData["server-ip"]);
            var authPort = config.GetValue<int>("auth-port");
            var apiPort = config.GetValue<int>("api-port");

            ApiProvider apiProvider = new ApiProvider(ip, authPort, apiPort);
               var api = await apiProvider.GetApiLogin("fuckus", "fuckus");
            api = await apiProvider.GetApiRegistration("fuckus", "fuckus", "fuckus");
           // var api = await apiProvider.GetApiLogin("fucku", "fucku");
            Console.WriteLine(api.Id + " " + api.UserInfo.MainInfo.Name);
            var i = await api.Users.GetUserInfo(api.Id);
            Console.WriteLine(i.MainInfo.Name);
            Console.Read();
        }
    }
}