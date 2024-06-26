﻿using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Chats;
using ApiTypes.Communication.Friends;
using TMApi.API;
using TMApi.ApiRequests.Chats;

namespace TMApi.ApiRequests.Friends
{
    public class Friends : BaseRequester
    {
        internal Friends(RequestSender requester, Api api) : base(requester, api)
        {
        }
        public async Task<FriendRequest?> GetFriendRequest(int requestId)
        {
            var result = await GetFriendRequest([requestId]);
            if (result.Length == 0)
                return null;
            return result[0];
        }
        public async Task<FriendRequest[]> GetFriendRequest(int[] requestIds)
        {
            return await RequestMany(requestIds,
                    (ids) => new GetFriendRequests(ids),
                    Requester.ApiRequestAsync<SerializableArray<FriendRequest>, GetFriendRequests>,
                    (x) => x.Id);
        }

        public async Task<int[]> GetAllRequests()
        {
            var requests = await Requester.ApiRequestAsync<IntArrayContainer, GetAllFriendRequests>(new GetAllFriendRequests());

            if (requests == null)
                return [];
            return requests.Values;
        }
        public async Task<bool> ResponseFriendRequest(int requestId, bool isAccepted)
        {
            return await Requester.ApiSendAsync(new RequestResponse(requestId, isAccepted));
        }
        public async Task<bool> SendFriendRequest(int toId)
        {
            return await Requester.ApiSendAsync(new FriendRequest(toId));
        }

        public async Task<bool> RemoveFriend(int friendId)
        {
            return await Requester.ApiSendAsync(new FriendRemoveRequest(friendId));
        }
    }
}
