using ApiTypes;
using CSDTP;

namespace TMServer.Logger
{
    internal interface ILogger
    {
        public void Log(string message);
        public void Log(string message, Exception exception);

        public void Log<T>(ApiData<T> apiData) where T : ISerializable<T>,new();
    }
}
