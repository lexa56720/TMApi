using ApiTypes;
using ApiTypes.Shared;
using Microsoft.Win32;
using System.Net;
using TMApi;

namespace TMClient
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            ApiProvider apiProvider = new ApiProvider(new IPAddress([127, 0, 0, 1]), 6665, 6666, 6667, TimeSpan.FromMinutes(3));
            await Register(apiProvider);

            var peterApi = await apiProvider.GetApiLogin("peter", "peter");
            await peterApi.Friends.SendFriendRequest(2);
            await peterApi.Friends.SendFriendRequest(3);
            await peterApi.Friends.SendFriendRequest(4);

            var ramsulApi = await apiProvider.GetApiLogin("ramsul", "ramsul");
            await ramsulApi.Friends.ResponseFriendRequest(1, true);

            var adminApi = await apiProvider.GetApiLogin("admin", "admin");
            await adminApi.Friends.ResponseFriendRequest(2, true);

            var mudakApi = await apiProvider.GetApiLogin("mudak", "mudak");
            await mudakApi.Friends.ResponseFriendRequest(3, true);

            Console.WriteLine("done");
        }

        private static async Task Register(ApiProvider apiProvider)
        {
            var peterApi = await apiProvider.GetApiRegistration("peter alexandros", "peter", "peter");
            var ramsulApi = await apiProvider.GetApiRegistration("ramsul abdulHalif", "ramsul", "ramsul");
            var adminApi = await apiProvider.GetApiRegistration("adminstarotr", "admin", "admin");
            var mudakApi = await apiProvider.GetApiRegistration("mudak", "mudak", "mudak");

        }
    }
}