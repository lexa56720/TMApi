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
                return await Requester.ApiSendAsync(new ChangeNameRequest(name));
            return false;
        }

        public async Task<UserInfo?> GetUserInfo()
        {
            return await Requester.ApiRequestAsync<UserInfo, UserFullRequest>(new UserFullRequest());
        }

        public async Task<User?> GetUser(int userId)
        {
            var result = await GetUser([userId]);
            if (result.Length == 0)
                return null;

            return result[0];
        }
        public async Task<User[]> GetUser(int[] userIds)
        {
            var users = await Requester.ApiRequestAsync<SerializableArray<User>, UserRequest>(new UserRequest(userIds));
            if (users == null)
                return [];

            return users.Items;
        }

        public async Task<User[]> GetByName(string name)
        {
            if (!DataConstraints.IsSearchQueryValid(name))
                return [];

            var users = await Requester.ApiRequestAsync<SerializableArray<User>, SearchRequest>(new SearchRequest(name));
            if (users == null)
                return [];

            return users.Items;
        }
    }
}
