using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Search;
using ApiTypes.Communication.Users;
using ApiTypes.Shared;

namespace TMApi.ApiRequests.Users
{
    public class Users : BaseRequester
    {
        internal Users(RequestSender requester, TMApi api) : base(requester, api)
        {
        }

        public async Task<bool> ChangeName(string name)
        {
            if (DataConstraints.IsNameLegal(name))
                return await Requester.GetRequestAsync
                    (RequestHeaders.ChangeName, new ChangeNameRequest(name));
            return false;
        }

        public async Task<UserInfo?> GetUserInfo(int userId)
        {
            return await Requester.PostRequestAsync<UserInfo, IntContainer>
                (RequestHeaders.GetUserInfo, new IntContainer(userId));
        }

        public async Task<User?> GetUser(int userId)
        {
            return await Requester.PostRequestAsync<User, IntContainer>
                (RequestHeaders.GetUser, new IntContainer(userId));
        }
        public async Task<User[]> GetUser(int[] userId)
        {
            var users = await Requester.PostRequestAsync<SerializableArray<User>, IntArrayContainer>
                (RequestHeaders.GetUserMany, new IntArrayContainer(userId));

            if (users == null)
                return Array.Empty<User>();
            return users.Items;
        }

        public async Task<User[]> GetByName(string name)
        {
            if (!DataConstraints.IsSearchQueryValid(name))
                return Array.Empty<User>();

            var users = await Requester.PostRequestAsync<SerializableArray<User>, SearchRequest>
                (RequestHeaders.SearchByName, new SearchRequest(name));
            return users.Items;
        }
    }
}
