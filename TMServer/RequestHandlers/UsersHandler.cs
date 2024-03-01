﻿using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Users;
using TMServer.DataBase.Interaction;
using TMServer.DataBase.Tables;

namespace TMServer.RequestHandlers
{
    internal static class UsersHandler
    {
        public static UserInfo? GetUserInfo(ApiData<UserFullRequest> id)
        {
            var user = Users.GetUserFull(id.UserId);
            if (user == null)
                return null;

            var friends = user.GetFriends().Select(ConvertUser).ToArray();

            return new UserInfo()
            {
                Chats = user.Chats.Select(c => c.Id).ToArray(),
                Friends = friends,
                FriendRequests = Friends.GetAllForUser(id.UserId),
                ChatInvites = Chats.GetAllChatInvites(id.UserId),
                MainInfo = ConvertUser(user),
            };
        }

        public static SerializableArray<User> GetUsers(ApiData<UserRequest> ids)
        {
            var users = Users.GetUserMain(ids.Data.Ids);
            if (users.Length == 0)
                return new SerializableArray<User>([]);
            return new SerializableArray<User>(users.Select(ConvertUser).ToArray());
        }

        public static void ChangeUserName(ApiData<ChangeNameRequest> request)
        {
            Users.ChangeName(request.UserId, request.Data.NewName);
        }

        private static User ConvertUser(DBUser dBUser)
        {
            return new User()
            {
                Name = dBUser.Name,
                Id = dBUser.Id,
                Login = dBUser.Login,
                IsOnline = dBUser.IsOnline
            };
        }
    }

}
