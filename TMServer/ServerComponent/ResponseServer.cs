using ApiTypes;
using CSDTP.Requests;
using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.ServerComponent.Basics;

namespace TMServer.Servers
{
    internal class ResponseServer : Server
    {
        private Dictionary<Type, Dictionary<string, object>> PostHandlers = new Dictionary<Type, Dictionary<string, object>>();


        private Dictionary<Type, Dictionary<string, object>> GetHandlers = new Dictionary<Type, Dictionary<string, object>>();

        public ResponseServer(int port) : base(port)
        {
        }


        public void RegisterGetHandler<T, U>(Func<ApiRequest<T>, U> func, string header) where T : ISerializable<T> where U : ISerializable<U>
        {
            if (!GetHandlers.ContainsKey(typeof(ApiRequest<T>)))
                GetHandlers.Add(typeof(ApiRequest<T>), new Dictionary<string, object>());

            if (GetHandlers[typeof(ApiRequest<T>)].ContainsKey(header))
                return;

            GetHandlers[typeof(ApiRequest<T>)].Add(header, func);
            Responder.RegisterGetHandler(new Action<ApiRequest<T>>(InvokeHandler<T>));
        }

        public void RegisterPostHandler<T, U>(Func<ApiRequest<T>, U> func, string header) where T : ISerializable<T> where U : ISerializable<U>
        {
            if (!PostHandlers.ContainsKey(typeof(ApiRequest<T>)))
                PostHandlers.Add(typeof(ApiRequest<T>), new Dictionary<string, object>());

            if (PostHandlers[typeof(ApiRequest<T>)].ContainsKey(header))
                return;

            PostHandlers[typeof(ApiRequest<T>)].Add(header, func);
            Responder.RegisterPostHandler(new Func<ApiRequest<T>, U>(InvokeHandler<T, U>));
        }
        private void InvokeHandler<T>(ApiRequest<T> request) where T : ISerializable<T>
        {
            if (PostHandlers.TryGetValue(typeof(ApiRequest<T>), out var typeHandler) && typeHandler.TryGetValue(request.Header, out var handler))
            {
                var result = ((Delegate)handler).Method.Invoke(handler, new object[] { request });
            }
        }
        private U? InvokeHandler<T, U>(ApiRequest<T> request) where T : ISerializable<T> where U : ISerializable<U>
        {
            if (PostHandlers.TryGetValue(typeof(ApiRequest<T>), out var typeHandler) && typeHandler.TryGetValue(request.Header, out var handler))
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
