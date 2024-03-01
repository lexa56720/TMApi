using ApiTypes;
using AutoSerializer;
using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.Logger
{
    internal class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void Log(string message, Exception exception)
        {
            Log(message+": "+exception.StackTrace);
        }

        public void Log<T>(ApiData<T> apiData) where T : ISerializable<T>,new()
        {
            Log($"{apiData.UserId} {apiData.Data.GetType().Name}");
        }
    }
}
