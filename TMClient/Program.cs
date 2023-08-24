using ApiTypes.Shared;
using TMApi;

namespace TMClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ApiProvider apiProvider = new ApiProvider();
            //var api = await apiProvider.Register("FFF666", "SSSS");
            var api = await apiProvider.GetApi("FFF666", "SSSS");
            Console.WriteLine(api.Id + " " + api.User.MainInfo.Name);
            var i = await api.Users.GetUserInfo(api.Id);
            Console.WriteLine(i.MainInfo.Name);
            Console.Read();
        }
    }
}