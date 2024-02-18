using ApiTypes;
using ApiTypes.Shared;
using System.Net;
using TMApi;

namespace TMClient
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {

            AutoSerializer.Serializer.SerializerProvider = new SerializerProvider();
            var config = new Configurator("config.cfg", true);

            var ip = IPAddress.Parse(config.ConfigData["server-ip"]);
            var authPort = config.GetValue<int>("auth-port");
            var apiPort = config.GetValue<int>("api-port");

            ApiProvider apiProvider = new ApiProvider(ip, authPort, apiPort, 6667,TimeSpan.FromMinutes(3));
           var api = await apiProvider.GetApiLogin("fuckusfd", "fuckusfd");
           // var api = await apiProvider.GetApiRegistration("fuckusfd", "fuckusfd", "fuckusfd");
            // var api = await apiProvider.GetApiLogin("fucku", "fucku");
            Console.WriteLine(api.Id + " " + api.UserInfo.MainInfo.Name);
            var i = await api.Users.GetUserInfo(api.Id);
            await api.Friends.SendFriendRequest(2);
            Console.WriteLine(i.MainInfo.Name);
            Console.Read();
        }
    }
}