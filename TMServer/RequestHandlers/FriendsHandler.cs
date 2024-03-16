using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Friends;
using ApiTypes;
using CSDTP.Requests;
using TMServer.DataBase.Tables;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TMServer.DataBase.Interaction;
using TMServer.DataBase;

namespace TMServer.RequestHandlers
{
    internal static class FriendsHandler
    {
        public static SerializableArray<FriendRequest>? GetFriendRequests(ApiData<GetFriendRequests> request)
        {
            if (!Security.IsHaveAccessToRequest(request.Data.Ids, request.UserId))
                return null;
            var requests = Friends.GetFriendRequest(request.Data.Ids);

            var filteredRequests = requests.Select(DbConverter.Convert).ToArray();

            if (filteredRequests.Length == 0)
                return new SerializableArray<FriendRequest>([]);

            return new SerializableArray<FriendRequest>(filteredRequests);
        }

        public static void AddFriendRequest(ApiData<FriendRequest> request)
        {
            if (Security.IsFriendshipPossible(request.UserId,request.Data.ToId))
                Friends.RegisterFriendRequest(request.UserId, request.Data.ToId);
        }
        public static void FriendRequestResponse(ApiData<RequestResponse> request)
        {
            if (Security.IsFriendRequestExist(request.Data.RequestId, request.UserId))
                Friends.FriendRequestResponse(request.Data.RequestId, request.Data.IsAccepted);
        }

        public static IntArrayContainer? GetAllFriendRequests(ApiData<GetAllFriendRequests> userId)
        {
            return new IntArrayContainer(Friends.GetAllForUser(userId.UserId));
        }

    }
}
