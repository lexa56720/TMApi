using TMApi;

namespace TMClient
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            ApiProvider apiProvider = new ApiProvider();
            //var api = await apiProvider.Register("FFF666", "SSSS");
            var api = await apiProvider.GetApiLogin("peter", "SSSS");
            Console.WriteLine(api.Id + " " + api.UserInfo.MainInfo.Name);
            var i = await api.Users.GetUserInfo(api.Id);
            Console.WriteLine(i.MainInfo.Name);
            Console.Read();
        }
    }
}