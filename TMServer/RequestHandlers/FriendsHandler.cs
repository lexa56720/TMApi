using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Friends;
using ApiTypes;
using CSDTP.Requests;
using TMServer.DataBase;
using TMServer.DataBase.Tables;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TMServer.RequestHandlers
{
    internal class FriendsHandler
    {
        public static FriendRequest? GetFriendRequest(ApiData<IntContainer> data)
        {
            var request = Friends.GetFriendRequest(data.Data.Value);
            if (request != null && IsHaveAccess(request, data.UserId))
                return new FriendRequest(request.UserOneId, request.UserTwoId);
            return null;
        }
        public static SerializableArray<FriendRequest> GetFriendRequests(ApiData<IntArrayContainer> data)
        {
            var requests = Friends.GetFriendRequest(data.Data.Values);

            var filteredRequests = requests.Where(r => IsHaveAccess(r, data.UserId))
                                           .Select(r => new FriendRequest(r.UserOneId, r.UserTwoId))
                                           .ToArray();

            return new SerializableArray<FriendRequest>(filteredRequests);
        }
        public static void AddFriendRequest(ApiData<FriendRequest> data)
        {
            Friends.RegisterFriendRequest(data.Data.FromId, data.Data.ToId);
        }
        public static void FriendRequestResponse(ApiData<FriendResponse> data)
        {
            Friends.FriendRequestResponse(data.Data.Request.FromId, data.Data.Request.ToId, data.Data.IsAccepted);
        }

        private static bool IsHaveAccess(DBFriendRequest request, int userId)
        {
            return request.UserOneId == userId || request.UserTwoId == userId;
        }
    }
}
