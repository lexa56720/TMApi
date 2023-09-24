using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Users;
using TMServer.DataBase.Interaction;

namespace TMServer.RequestHandlers
{
    internal static class UsersHandler
    {
        public static UserInfo? GetUserInfo(ApiData<IntContainer> id)
        {
            if(id.UserId!=id.Data.Value) 
                return null;

            var user = Users.GetUserFull(id.UserId);
            if (user == null)
                return null;

            var friends = user.GetFriends()
                .Select(c => new User(c.Name, c.Id, c.Login,c.IsOnline)).ToArray();

            return new UserInfo()
            {
                Chats = user.Chats.Select(c => c.Id).ToArray(),
                Friends = friends,
                FriendRequests=Friends.GetAllForUser(id.UserId),
                ChatInvites=Chats.GetAllChatInvites(id.UserId),
                MainInfo = new User(user.Name, user.Id, user.Login, user.IsOnline),
            };
        }

        public static User? GetUser(ApiData<IntContainer> id)
        {
            var user = Users.GetUserMain(id.Data.Value);
            if (user == null)
                return null;

            return new User(user.Name, user.Id, user.Login,user.IsOnline);
        }

        public static SerializableArray<User> GetUsers(ApiData<IntArrayContainer> ids)
        {
            var users = Users.GetUserMain(ids.Data.Values);
            if (!users.Any())
                return new SerializableArray<User>(Array.Empty<User>());
            return new SerializableArray<User>(users.Select(
                u => new User
                {
                    Name = u.Name,
                    Id = u.Id,
                    Login = u.Login,
                    IsOnline = u.IsOnline
                }).ToArray());
        }

        public static void ChangeUserName(ApiData<ChangeNameRequest> request)
        {
            Users.ChangeName(request.UserId, request.Data.NewName);
        }
    }

}
