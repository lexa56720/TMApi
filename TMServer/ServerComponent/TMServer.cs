using ApiTypes;
using ApiTypes.Communication.Auth;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Users;
using TMServer.RequestHandlers;
using TMServer.ServerComponent.ApiResponser;
using TMServer.ServerComponent.Auth;
using TMServer.ServerComponent.Basics;

namespace TMServer.Servers
{
    internal class TMServer : Startable, IDisposable
    {
        private AuthorizationServer AuthServer { get; set; }

        private ResponseServer ResponseServer { get; set; }

        public TMServer(int authPort, int responsePort)
        {
            AuthServer = new AuthorizationServer(authPort, new AuthEncryptProvider());
            ResponseServer = new ResponseServer(responsePort, new ApiEncryptProvider());
            RegisterAuthMethods();
            RegisterApiMethods();
        }

        public void RegisterAuthMethods()
        {
            AuthServer.Register<RsaPublicKey, RsaPublicKey>(AuthHandler.RsaKeyTrade);
            AuthServer.Register<AuthorizationRequest, AuthorizationResponse>(AuthHandler.Auth);
            AuthServer.Register<RegisterRequest, RegisterResponse>(AuthHandler.Register);
        }

        public void RegisterApiMethods()
        {
            ResponseServer.RegisterPostHandler<IntContainer, UserInfo>(UsersHandler.GetUserInfo, RequestHeaders.GetUserInfo);
            ResponseServer.RegisterPostHandler<IntContainer, User>(UsersHandler.GetUser, RequestHeaders.GetUser);
            ResponseServer.RegisterPostHandler<IntArrayContainer, SerializableArray<User>>(UsersHandler.GetUser, RequestHeaders.GetUserMany);
            ResponseServer.RegisterPostHandler<AuthUpdateRequest, AuthorizationResponse>(AuthHandler.UpdateAuth, RequestHeaders.UpdateAuth);

        }
        public override void Start()
        {
            base.Start();
            AuthServer.Start();
            ResponseServer.Start();
        }

        public override void Stop()
        {
            base.Stop();
            AuthServer.Stop();
            ResponseServer.Stop();
        }
        public void Dispose()
        {
            AuthServer.Dispose();
        }
    }
}
