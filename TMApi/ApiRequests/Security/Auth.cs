using ApiTypes;
using ApiTypes.Communication.Auth;

namespace TMApi.ApiRequests.Security
{
    public class Auth : BaseRequester
    {
        internal Auth(RequestSender requester, Api api) : base(requester, api)
        {
        }

        public async Task<AuthorizationResponse?> UpdateAuth()
        {
            return await Requester.ApiRequestAsync<AuthorizationResponse, AuthUpdateRequest>(new AuthUpdateRequest());
        }
    }
}
