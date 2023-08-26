using ApiTypes;
using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.Logger
{
    internal interface ILogger
    {

        public void Log(string message);
        public void Log(string message, Exception exception);

        public void Log<T>(ApiData<T> apiData) where T:ISerializable<T>
        {

        }

    }
}
