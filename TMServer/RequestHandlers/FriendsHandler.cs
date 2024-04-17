using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Friends;
using ApiTypes;
using CSDTP.Requests;
using TMServer.DataBase.Tables;
using TMServer.DataBase.Interaction;
using TMServer.DataBase;

namespace TMServer.RequestHandlers
{
    public class FriendsHandler
    {
        private readonly Security Security;
        private readonly Friends Friends;
        private readonly DbConverter Converter;

        public FriendsHandler(Security security, Friends friends, DbConverter converter)
        {
            Security = security;
            Friends = friends;
            Converter = converter;
        }
        public async Task<SerializableArray<FriendRequest>?> GetFriendRequests(ApiData<GetFriendRequests> request)
        {
            if (!await Security.IsHaveAccessToRequest(request.UserId, request.Data.Ids))
                return null;

            var converted = Converter.Convert((await Friends.GetFriendRequest(request.Data.Ids)).ToArray());
            if (converted.Length == 0)
                return new SerializableArray<FriendRequest>([]);

            return new SerializableArray<FriendRequest>(converted);
        }

        public async Task AddFriendRequest(ApiData<FriendRequest> request)
        {
            if (!await Security.IsFriendshipPossible(request.UserId, request.Data.ToId))
                return;

            if (await Security.IsExistOppositeRequest(request.UserId, request.Data.ToId))
            {
                await Friends.RemoveFriendRequest(request.Data.ToId, request.UserId);
                await Friends.RegisterFriends(request.UserId, request.Data.ToId);
            }
            else
                await Friends.RegisterFriendRequest(request.UserId, request.Data.ToId);
        }
        public async Task FriendRequestResponse(ApiData<RequestResponse> request)
        {
            if (!await Security.IsExistFriendRequest(request.Data.RequestId, request.UserId))
                return;

            var dbRequest = await Friends.RemoveFriendRequest(request.Data.RequestId);
            if (request.Data.IsAccepted)
                await Friends.RegisterFriends(dbRequest.SenderId, dbRequest.ReceiverId);
        }

        public async Task<IntArrayContainer?> GetAllFriendRequests(ApiData<GetAllFriendRequests> request)
        {
            return new IntArrayContainer(await Friends.GetAllForUser(request.UserId));
        }

        internal async Task RemoveFriend(ApiData<FriendRemoveRequest> request)
        {
            if (!await Security.IsFriends(request.UserId, request.Data.FriendId))
                return;
            await Friends.RemoveFriend(request.UserId, request.Data.FriendId);
        }
    }
}
