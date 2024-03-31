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
        private readonly Images Images;
        private readonly DbConverter Converter;

        public UsersHandler(Users users, Chats chats, Friends friends, Images images, DbConverter converter)
        {
            Users = users;
            Chats = chats;
            Friends = friends;
            Images = images;
            Converter = converter;
        }
        public UserInfo? GetUserInfo(ApiData<UserFullRequest> id)
        {
            var user = Users.GetUserFull(id.UserId);
            if (user == null)
                return null;

            var friends = Converter.Convert(user.GetFriends().ToArray());

            return new UserInfo()
            {
                Chats = user.Chats.Select(c => c.Id).ToArray(),
                Friends = friends,
                FriendRequests = Friends.GetAllForUser(id.UserId),
                ChatInvites = Chats.GetAllChatInvites(id.UserId),
                MainInfo = Converter.Convert(user),
            };
        }

        public SerializableArray<User> GetUsers(ApiData<UserRequest> ids)
        {
            var users = Users.GetUserMain(ids.Data.Ids);
            if (users.Length == 0)
                return new SerializableArray<User>([]);
            return new SerializableArray<User>(Converter.Convert(users));
        }

        public User? ChangeUserName(ApiData<ChangeNameRequest> request)
        {
            if (!DataConstraints.IsNameLegal(request.Data.NewName))
                return null;
            var user = Users.ChangeName(request.UserId, request.Data.NewName);
            if (user == null)
                return null;
            return Converter.Convert(user, null);
        }
    }
}
