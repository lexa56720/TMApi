using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Friends;

namespace TMApi.ApiRequests.Friends
{
    public class Friends : BaseRequester
    {
        internal Friends(RequestSender requester, Api api) : base(requester, api)
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
                (RequestHeaders.ResponseFriendRequest, new RequestResponse(request.Id, isAccepted));
        }
        public async Task<bool> SendFriendRequest(int toId)
        {
            return await Requester.GetRequestAsync
                (RequestHeaders.SendFriendRequest, new FriendRequest(Api.Id, toId));
        }
    }
}
