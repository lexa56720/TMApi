using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Users;
using TMServer.DataBase;

namespace TMServer.RequestHandlers
{
    internal static class UsersHandler
    {
        public static UserInfo? GetUserInfo(ApiData<IntContainer> id)
        {
            var user = Users.GetUserFull(id.UserId);
            if (user == null)
                return null;

            var friends = user.FriendsOne
                .Concat(user.FriendsTwo)
                .Select(f =>
                {
                    if (f.UserIdOne == user.Id)
                        return f.UserOne;
                    return f.UserTwo;
                }).Select(c => new User(c.Name, c.Id, c.IsOnline)).ToArray();

            return new UserInfo()
            {
                Chats = user.Chats.Select(c => c.Id).ToArray(),
                Friends = friends,
                MainInfo = new User(user.Name, user.Id, true),
            };
        }

        public static User? GetUser(ApiData<IntContainer> id)
        {
            var user = Users.GetUserMain(id.Data.Value);
            if (user == null)
                return null;

            return new User(user.Name, user.Id, user.IsOnline);
        }

        public static SerializableArray<User> GetUsers(ApiData<IntArrayContainer> ids)
        {
            var users = Users.GetUserMain(ids.Data.Values);

            return new SerializableArray<User>(users.Select(u => new User(u.Name, u.Id, u.IsOnline)).ToArray());
        }

        public static void ChangeUserName(ApiData<ChangeNameRequest> request)
        {
            Users.ChangeName(request.UserId, request.Data.NewName);
        }
    }

}
