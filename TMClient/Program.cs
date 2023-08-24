using TMApi;

namespace TMClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ApiProvider apiProvider = new ApiProvider();
            var api = await apiProvider.Register("FFF666", "SSSS");
            Console.WriteLine(api.Id + " " + api.User.MainInfo.Name);
            Console.Read();
        }
    }
}