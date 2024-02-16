using ApiTypes;
using CSDTP;
using CSDTP.Cryptography.Providers;
using CSDTP.Requests;
using TMServer.DataBase;
using TMServer.DataBase.Interaction;
using TMServer.Logger;
using TMServer.ServerComponent.Basics;

namespace TMServer.ServerComponent.ApiResponser
{
    internal class ResponseServer : Server
    {
        private Dictionary<Type, Dictionary<RequestHeaders, object>> PostHandlers = [];


        private Dictionary<Type, Dictionary<RequestHeaders, object>> GetHandlers = [];

        public ResponseServer(int port, IEncryptProvider encryptProvider, ILogger logger) : base(port, encryptProvider, logger)
        {
        }

        public void RegisterDataHandler<TRequest>(Action<ApiData<TRequest>> func, RequestHeaders header) 
                    where TRequest : ISerializable<TRequest>, new()
        {
            var type = typeof(ApiData<TRequest>);
            if (!GetHandlers.ContainsKey(type))
            {
                GetHandlers.Add(type, new Dictionary<RequestHeaders, object>());
                Responder.RegisterDataHandler(new Action<ApiData<TRequest>>(InvokeHandler<TRequest>));
            }

            if (GetHandlers.TryGetValue(type, out var handler) && handler.ContainsKey(header))
                return;

            GetHandlers[type].Add(header, func);
        }
        public void RegisterRequestHandler<TRequest, TResponse>(Func<ApiData<TRequest>, TResponse?> func, RequestHeaders header) 
                    where TRequest : ISerializable<TRequest>, new() 
                    where TResponse : ISerializable<TResponse>, new()
        {
            var type = typeof((ApiData<TRequest>,TResponse));
            if (!PostHandlers.ContainsKey(type))
            {
                PostHandlers.Add(type, []);
                Responder.RegisterRequestHandler(new Func<ApiData<TRequest>, TResponse?>(InvokeHandler<TRequest, TResponse>));
            }

            if (PostHandlers.TryGetValue(type, out var handler) && handler.ContainsKey(header))
                return;

            PostHandlers[type].Add(header, func);
        }

        private void InvokeHandler<TRequest>(ApiData<TRequest> request) where TRequest : ISerializable<TRequest>, new()
        {
            if (IsRequestLegal(request) && GetHandlers.TryGetValue(typeof(ApiData<TRequest>),
                out var typeHandler) && typeHandler.TryGetValue(request.Header, out var handler))
            {
                Logger.Log(request);
                ((Delegate)handler).Method.Invoke(handler, new object[] { request });
            }
        }
        private TResponse? InvokeHandler<TRequest, TResponse>(ApiData<TRequest> request) 
                           where TRequest : ISerializable<TRequest>, new() 
                           where TResponse : ISerializable<TResponse>, new()
        {
            if (IsRequestLegal(request) && PostHandlers.TryGetValue(typeof((ApiData<TRequest>, TResponse)),
                out var typeHandler) && typeHandler.TryGetValue(request.Header, out var handler))
            {
                Logger.Log(request);
                return (TResponse?)((Delegate)handler).Method.Invoke(handler, new object[] { request });
            }

            return default;
        }

    }
}
