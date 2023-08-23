using ApiTypes;
using CSDTP.Requests;
using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.ServerComponent.Basics;
using CSDTP.Cryptography.Providers;

namespace TMServer.ServerComponent.ApiResponser
{
    internal class ResponseServer : Server
    {
        private Dictionary<Type, Dictionary<string, object>> PostHandlers = new Dictionary<Type, Dictionary<string, object>>();


        private Dictionary<Type, Dictionary<string, object>> GetHandlers = new Dictionary<Type, Dictionary<string, object>>();

        public ResponseServer(int port,IEncryptProvider encryptProvider) : base(port, encryptProvider)
        {
        }


        public void RegisterGetHandler<T, U>(Func<ApiData<T>, U> func, string header) where T : ISerializable<T> where U : ISerializable<U>
        {
            if (!GetHandlers.ContainsKey(typeof(ApiData<T>)))
                GetHandlers.Add(typeof(ApiData<T>), new Dictionary<string, object>());

            if (GetHandlers[typeof(ApiData<T>)].ContainsKey(header))
                return;

            GetHandlers[typeof(ApiData<T>)].Add(header, func);
            Responder.RegisterGetHandler(new Action<ApiData<T>>(InvokeHandler<T>));
        }

        public void RegisterPostHandler<T, U>(Func<ApiData<T>, U> func, string header) where T : ISerializable<T> where U : ISerializable<U>
        {
            if (!PostHandlers.ContainsKey(typeof(ApiData<T>)))
                PostHandlers.Add(typeof(ApiData<T>), new Dictionary<string, object>());

            if (PostHandlers[typeof(ApiData<T>)].ContainsKey(header))
                return;

            PostHandlers[typeof(ApiData<T>)].Add(header, func);
            Responder.RegisterPostHandler(new Func<ApiData<T>, U>(InvokeHandler<T, U>));
        }
        private void InvokeHandler<T>(ApiData<T> request) where T : ISerializable<T>
        {
            if (PostHandlers.TryGetValue(typeof(ApiData<T>), out var typeHandler) && typeHandler.TryGetValue(request.Header, out var handler))
            {
                var result = ((Delegate)handler).Method.Invoke(handler, new object[] { request });
            }
        }
        private U? InvokeHandler<T, U>(ApiData<T> request) where T : ISerializable<T> where U : ISerializable<U>
        {
            if (PostHandlers.TryGetValue(typeof(ApiData<T>), out var typeHandler) && typeHandler.TryGetValue(request.Header, out var handler))
            {
                var result = ((Delegate)handler).Method.Invoke(handler, new object[] { request });
                return (U)result;
            }
            else
            {
                return default;
            }
        }
    }
}
