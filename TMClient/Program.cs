using ApiTypes;
using ApiTypes.Shared;
using Microsoft.Win32;
using System.Net;
using TMApi.API;

namespace TMClient
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var apiProvider = await ApiProvider.CreateProvider(new IPAddress([127, 0, 0, 1]), 6660);
            if (apiProvider == null)
                return;

            var peterApi = await apiProvider.GetApiRegistration("peter alexandros", "peter", "peter");
            var ramsulApi = await apiProvider.GetApiRegistration("ramsul abdulHalif", "ramsul", "ramsul");
            var adminApi = await apiProvider.GetApiRegistration("administrator", "admin", "admin");
            var ivanApi = await apiProvider.GetApiRegistration("ivan", "ivan", "ivan");

            if (peterApi == null || ramsulApi == null || ivanApi == null || adminApi == null)
            {
                Console.WriteLine("error");
                return;
            }

            await peterApi.Friends.SendFriendRequest(2);
            await peterApi.Friends.SendFriendRequest(3);
            await peterApi.Friends.SendFriendRequest(4);

            await AcceptAll(ramsulApi);
            await AcceptAll(ivanApi);
            await AcceptAll(adminApi);

            Console.WriteLine("done");
            Console.ReadLine();
        }

        private static async Task AcceptAll(Api api)
        {
            int[] requests = await api.Friends.GetAllRequests();
            foreach (var request in requests)
            {
                await api.Friends.ResponseFriendRequest(request, true);
            }
        }

        private static async Task Register(ApiProvider apiProvider)
        {
            using var peterApi = await apiProvider.GetApiRegistration("peter alexandros", "peter", "peter");
            using var ramsulApi = await apiProvider.GetApiRegistration("ramsul abdulHalif", "ramsul", "ramsul");
            using var adminApi = await apiProvider.GetApiRegistration("adminstarotr", "admin", "admin");
            using var mudakApi = await apiProvider.GetApiRegistration("ivan", "ivan", "ivan");
        }
    }
}