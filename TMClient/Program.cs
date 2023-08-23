using TMApi;

namespace TMClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ApiProvider apiProvider = new ApiProvider();
            var api = await apiProvider.Register("FFF", "SSSS");

        }
    }
}