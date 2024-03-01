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
            var result = await GetFriendRequest([requestId]);
            if (result.Length == 0)
                return null;
            return result[0];
        }
        public async Task<FriendRequest[]> GetFriendRequest(int[] requestIds)
        {
            var requests = await Requester.RequestAsync<SerializableArray<FriendRequest>, IntArrayContainer>
                                    (RequestHeaders.GetFriendRequest, new IntArrayContainer(requestIds));

            if (requests == null)
                return [];
            return requests.Items;
        }

        public async Task<int[]> GetAllRequests()
        {
            var requests = await Requester.RequestAsync<IntArrayContainer, Request>
                (RequestHeaders.GetAllFriendRequests, new Request());

            if (requests == null)
                return [];
            return requests.Values;
        }
        public async Task<bool> ResponseFriendRequest(int requestId, bool isAccepted)
        {
            return await Requester.SendAsync
                (RequestHeaders.ResponseFriendRequest, new RequestResponse(requestId, isAccepted));
        }
        public async Task<bool> SendFriendRequest(int toId)
        {
            return await Requester.SendAsync
                (RequestHeaders.SendFriendRequest, new FriendRequest(Api.Id, toId));
        }
    }
}
