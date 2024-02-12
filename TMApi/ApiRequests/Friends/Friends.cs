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
            return await Requester.PostAsync<FriendRequest, IntContainer>
                (RequestHeaders.GetFriendRequest, new IntContainer(requestId));
        }
        public async Task<FriendRequest[]> GetFriendRequest(int[] requestIds)
        {
            var requests = await Requester
                .PostAsync<SerializableArray<FriendRequest>, IntArrayContainer>
                (RequestHeaders.GetFriendRequestMany, new IntArrayContainer(requestIds));

            if (requests == null)
                return [];
            return requests.Items;
        }

        public async Task<int[]> GetAllRequests(int userId)
        {
            var requests = await Requester.PostAsync<IntArrayContainer, IntContainer>
                (RequestHeaders.GetAllFriendRequestForUser, new IntContainer(userId));

            if (requests == null)
                return [];
            return requests.Values;
        }
        public async Task<bool> ResponseFriendRequest(FriendRequest request, bool isAccepted)
        {
            return await ResponseFriendRequest(request.Id, isAccepted);
        }
        public async Task<bool> ResponseFriendRequest(int requestId, bool isAccepted)
        {
            return await Requester.GetAsync
                (RequestHeaders.ResponseFriendRequest, new RequestResponse(requestId, isAccepted));
        }
        public async Task<bool> SendFriendRequest(int toId)
        {
            return await Requester.GetAsync
                (RequestHeaders.SendFriendRequest, new FriendRequest(Api.Id, toId));
        }
    }
}
