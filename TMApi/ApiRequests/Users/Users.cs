using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Search;
using ApiTypes.Communication.Users;
using ApiTypes.Shared;

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
                return await Requester.GetAsync
                    (RequestHeaders.ChangeName, new ChangeNameRequest(name));
            return false;
        }

        public async Task<UserInfo?> GetUserInfo(int userId)
        {
            return await Requester.PostAsync<UserInfo, IntContainer>
                (RequestHeaders.GetUserInfo, new IntContainer(userId));
        }

        public async Task<User?> GetUser(int userId)
        {
            return await Requester.PostAsync<User, IntContainer>
                (RequestHeaders.GetUser, new IntContainer(userId));
        }
        public async Task<User[]> GetUser(int[] userId)
        {
            var users = await Requester.PostAsync<SerializableArray<User>, IntArrayContainer>
                (RequestHeaders.GetUserMany, new IntArrayContainer(userId));

            if (users == null)
                return Array.Empty<User>();
            return users.Items;
        }

        public async Task<User[]> GetByName(string name)
        {
            if (!DataConstraints.IsSearchQueryValid(name))
                return Array.Empty<User>();

            var users = await Requester.PostAsync<SerializableArray<User>, SearchRequest>
                (RequestHeaders.SearchByName, new SearchRequest(name));
            if (users == null)
                return Array.Empty<User>();

            return users.Items;
        }
    }
}
