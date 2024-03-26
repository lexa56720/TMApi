using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Medias;
using ApiTypes.Communication.Messages;
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

        public async Task<bool> SetProfileImage(byte[] imageData)
        {
            if (imageData.Length == 0)
                return false;

            return await Requester.SendFileAsync(new ChangeUserProfileImageRequest(imageData));
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
            return await RequestMany(userIds,
                        (ids) => new UserRequest(ids),
                        Requester.ApiRequestAsync<SerializableArray<User>, UserRequest>,
                        (x) => x.Id);
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
