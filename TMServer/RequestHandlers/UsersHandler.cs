using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Users;
using ApiTypes.Shared;
using TMServer.DataBase.Interaction;
using TMServer.DataBase.Tables;

namespace TMServer.RequestHandlers
{
    public class UsersHandler
    {
        private readonly Users Users;
        private readonly Chats Chats;
        private readonly Friends Friends;
        private readonly DbConverter Converter;

        public UsersHandler(Users users, Chats chats, Friends friends, DbConverter converter)
        {
            Users = users;
            Chats = chats;
            Friends = friends;
            Converter = converter;
        }
        public async Task<UserInfo?> GetUserInfo(ApiData<UserFullRequest> id)
        {
            var user = await Users.GetUserWithFriends(id.UserId);
            if (user == null)
                return null;

            return new UserInfo()
            {
                Chats = user.Chats.Select(c => c.Id).ToArray(),
                Friends = await Converter.Convert(user.GetFriends().ToArray()),
                FriendRequests = await Friends.GetAllForUser(id.UserId),
                ChatInvites = await Chats.GetAllChatInvites(id.UserId),
                MainInfo = await Converter.Convert(user),
            };
        }

        public async Task<SerializableArray<User>> GetUsers(ApiData<UserRequest> ids)
        {
            var users =await Users.GetUserMain(ids.Data.Ids);
            if (users.Length == 0)
                return new SerializableArray<User>([]);
            return new SerializableArray<User>(await Converter.Convert(users));
        }

        public async Task<User?> ChangeUserName(ApiData<ChangeNameRequest> request)
        {
            if (!DataConstraints.IsNameLegal(request.Data.NewName))
                return null;
            var user =await Users.ChangeName(request.UserId, request.Data.NewName);
            if (user == null)
                return null;
            return Converter.Convert(user, null);
        }
    }
}
