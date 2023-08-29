﻿using ApiTypes;
using ApiTypes.Communication.Auth;

namespace TMApi.ApiRequests.Security
{
    public class Auth : BaseRequester
    {
        internal Auth(RequestSender requester, TMApi api) : base(requester, api)
        {
        }

        public async Task<AuthorizationResponse?> UpdateAuth()
        {
            return await Requester.PostRequestAsync<AuthorizationResponse, AuthUpdateRequest>(RequestHeaders.UpdateAuth, new AuthUpdateRequest());
        }
    }
}