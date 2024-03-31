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
            ApiProvider apiProvider = new ApiProvider(new IPAddress([127, 0, 0, 1]), 6665, 6666, 6667,6668, TimeSpan.FromMinutes(3));

            var peterApi = await apiProvider.GetApiRegistration("peter alexandros", "peter", "peter");
            var ramsulApi = await apiProvider.GetApiRegistration("ramsul abdulHalif", "ramsul", "ramsul");
            var adminApi = await apiProvider.GetApiRegistration("adminstarotr", "admin", "admin");
            var mudakApi = await apiProvider.GetApiRegistration("mudak", "mudak", "mudak");

            await peterApi.Friends.SendFriendRequest(2);
            await peterApi.Friends.SendFriendRequest(3);
            await peterApi.Friends.SendFriendRequest(4);


            await AcceptAll(ramsulApi);
            await AcceptAll(mudakApi);
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
            var peterApi = await apiProvider.GetApiRegistration("peter alexandros", "peter", "peter");
            var ramsulApi = await apiProvider.GetApiRegistration("ramsul abdulHalif", "ramsul", "ramsul");
            var adminApi = await apiProvider.GetApiRegistration("adminstarotr", "admin", "admin");
            var mudakApi = await apiProvider.GetApiRegistration("mudak", "mudak", "mudak");

        }
    }
}