﻿using ApiTypes;
using ApiTypes.BaseTypes;
using ApiTypes.Friends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMApi.ApiRequests.Friends
{
    public class Friends : BaseRequester
    {
        internal Friends(RequestSender requester, TMApi api) : base(requester, api)
        {
        }
        public async Task<FriendRequest?> GetFriendRequest(int requestId)
        {
            return await Requester.PostRequestAsync<FriendRequest, IntContainer>
                (RequestHeaders.GetFriendRequest, new IntContainer(requestId));
        }
        public async Task<FriendRequest[]> GetFriendRequest(int[] requestIds)
        {
            var requests = await Requester
                .PostRequestAsync<SerializableArray<FriendRequest>, IntArrayContainer>
                (RequestHeaders.GetFriendRequestMany, new IntArrayContainer(requestIds));

            if (requests == null)
                return Array.Empty<FriendRequest>();
            return requests.Items;
        }
        public async Task<bool> ResponseFriendRequest(FriendRequest request, bool isAccepted)
        {
            return await Requester.GetRequestAsync
                (RequestHeaders.ResponseFriendRequest, new FriendResponse(request, isAccepted));
        }
        public async Task<bool> SendFriendRequest(int toId)
        {
            return await Requester.GetRequestAsync
                (RequestHeaders.SendFriendRequest, new FriendRequest(Api.Id, toId));
        }
    }
}