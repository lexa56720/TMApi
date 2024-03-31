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
        public SerializableArray<FriendRequest>? GetFriendRequests(ApiData<GetFriendRequests> request)
        {
            if (!Security.IsHaveAccessToRequest(request.UserId, request.Data.Ids))
                return null;

            var converted = Converter.Convert(Friends.GetFriendRequest(request.Data.Ids).ToArray());
            if (converted.Length == 0)
                return new SerializableArray<FriendRequest>([]);

            return new SerializableArray<FriendRequest>(converted);
        }

        public void AddFriendRequest(ApiData<FriendRequest> request)
        {
            if (!Security.IsFriendshipPossible(request.UserId, request.Data.ToId))
                return;

            if (Security.IsExistOppositeRequest(request.UserId, request.Data.ToId))
            {
                Friends.RemoveFriendRequest(request.Data.ToId, request.UserId);
                Friends.RegisterFriends(request.UserId, request.Data.ToId);
            }
            else
                Friends.RegisterFriendRequest(request.UserId, request.Data.ToId);
        }
        public void FriendRequestResponse(ApiData<RequestResponse> request)
        {
            if (!Security.IsExistFriendRequest(request.Data.RequestId, request.UserId))
                return;

            var dbRequest = Friends.RemoveFriendRequest(request.Data.RequestId);
            if (request.Data.IsAccepted)
                Friends.RegisterFriends(dbRequest.SenderId, dbRequest.ReceiverId);
        }

        public IntArrayContainer? GetAllFriendRequests(ApiData<GetAllFriendRequests> request)
        {
            return new IntArrayContainer(Friends.GetAllForUser(request.UserId));
        }

        internal void RemoveFriend(ApiData<FriendRemoveRequest> request)
        {
            if (!Security.IsFriends(request.UserId, request.Data.FriendId))
                return;
            Friends.RemoveFriend(request.UserId, request.Data.FriendId);
        }
    }
}
