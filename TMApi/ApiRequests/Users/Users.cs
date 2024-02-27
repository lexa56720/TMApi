using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Search;
using ApiTypes.Communication.Users;
using ApiTypes.Shared;
using TMApi.ApiRequests.Messages;

namespace TMApi.ApiRequests.Users
{
    public class Users : BaseRequester
    {
        internal Users(RequestSender requester, Api api) : base(requester, api)
        {
        }

        public async Task<bool> ChangeName(string name)
        {
            if (DataConstraints.IsNameLegal(name))
                return await Requester.SendAsync
                    (RequestHeaders.ChangeName, new ChangeNameRequest(name));
            return false;
        }

        public async Task<UserInfo?> GetUserInfo()
        {
            return await Requester.RequestAsync<UserInfo, Request>
                        (RequestHeaders.GetUserInfo, new Request());
        }

        public async Task<User?> GetUser(int userId)
        {
            return await Requester.RequestAsync<User, IntContainer>
                (RequestHeaders.GetUser, new IntContainer(userId));
        }
        public async Task<User[]> GetUser(int[] userIds)
        {
            if (userIds.Length == 0)
                return [];
            var users = await Requester.RequestAsync<SerializableArray<User>, IntArrayContainer>
                (RequestHeaders.GetUserMany, new IntArrayContainer(userIds));

            if (users == null)
                return [];
            return users.Items;
        }

        public async Task<User[]> GetByName(string name)
        {
            if (!DataConstraints.IsSearchQueryValid(name))
                return [];

            var users = await Requester.RequestAsync<SerializableArray<User>, SearchRequest>
                (RequestHeaders.SearchByName, new SearchRequest(name));
            if (users == null)
                return [];

            return users.Items;
        }
    }
}
