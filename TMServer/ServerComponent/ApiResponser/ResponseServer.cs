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
using TMServer.DataBase;

namespace TMServer.ServerComponent.ApiResponser
{
    internal class ResponseServer : Server
    {
        private Dictionary<Type, Dictionary<RequestHeaders, object>> PostHandlers = new();


        private Dictionary<Type, Dictionary<RequestHeaders, object>> GetHandlers = new();

        public ResponseServer(int port, IEncryptProvider encryptProvider) : base(port, encryptProvider)
        {
        }

        public void RegisterGetHandler<T, U>(Func<ApiData<T>, U> func, RequestHeaders header) where T : ISerializable<T> where U : ISerializable<U>
        {
            var type = typeof(ApiData<T>);
            if (!GetHandlers.ContainsKey(type))
                GetHandlers.Add(type, new Dictionary<RequestHeaders, object>());

            if (GetHandlers[type].ContainsKey(header))
                return;

            GetHandlers[type].Add(header, func);
            Responder.RegisterGetHandler(new Action<ApiData<T>>(InvokeHandler<T>));
        }
        public void RegisterPostHandler<T, U>(Func<ApiData<T>, U> func, RequestHeaders header) where T : ISerializable<T> where U : ISerializable<U>
        {
            var type = typeof(ApiData<T>);
            if (!PostHandlers.ContainsKey(type))
                PostHandlers.Add(type, new Dictionary<RequestHeaders, object>());

            if (PostHandlers[type].ContainsKey(header))
                return;

            PostHandlers[type].Add(header, func);
            Responder.RegisterPostHandler(new Func<ApiData<T>, U>(InvokeHandler<T, U>));
        }

        private void InvokeHandler<T>(ApiData<T> request) where T : ISerializable<T>
        {
            if (IsRequestLegal(request) && PostHandlers.TryGetValue(typeof(ApiData<T>),
                out var typeHandler) && typeHandler.TryGetValue(request.Header, out var handler))

                ((Delegate)handler).Method.Invoke(handler, new object[] { request });
        }
        private U? InvokeHandler<T, U>(ApiData<T> request) where T : ISerializable<T> where U : ISerializable<U>
        {
            if (IsRequestLegal(request) && PostHandlers.TryGetValue(typeof(ApiData<T>),
                out var typeHandler) && typeHandler.TryGetValue(request.Header, out var handler))
                return (U)((Delegate)handler).Method.Invoke(handler, new object[] { request });

            return default;
        }

        private bool IsRequestLegal<T>(ApiData<T> request) where T : ISerializable<T>
        {
            return Security.IsTokenCorrect(request.Token, request.UserId);
        }
    }
}
