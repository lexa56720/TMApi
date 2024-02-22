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
    internal class FriendsHandler
    {
        public static FriendRequest? GetFriendRequest(ApiData<IntContainer> data)
        {
            var request = Friends.GetFriendRequest(data.Data.Value);
            if (request != null && IsHaveAccess(request, data.UserId))
                return new FriendRequest(request.SenderId, request.ReceiverId, request.Id);
            return null;
        }
        public static SerializableArray<FriendRequest> GetFriendRequests(ApiData<IntArrayContainer> data)
        {
            var requests = Friends.GetFriendRequest(data.Data.Values);

            var filteredRequests = requests.Where(r => IsHaveAccess(r, data.UserId))
                                           .Select(r => new FriendRequest(r.SenderId, r.ReceiverId, r.Id))
                                           .ToArray();

            if (!filteredRequests.Any())
                return new SerializableArray<FriendRequest>([]);

            return new SerializableArray<FriendRequest>(filteredRequests);
        }

        public static void AddFriendRequest(ApiData<FriendRequest> data)
        {
            Friends.RegisterFriendRequest(data.Data.FromId, data.Data.ToId);
        }
        public static void FriendRequestResponse(ApiData<RequestResponse> data)
        {
            Friends.FriendRequestResponse(data.Data.RequestId, data.Data.IsAccepted);
        }

        public static IntArrayContainer? GetAllFriendRequests(ApiData<Request> userId)
        {
            return new IntArrayContainer(Friends.GetAllForUser(userId.UserId));
        }
        private static bool IsHaveAccess(DBFriendRequest request, int userId)
        {
            return request.SenderId == userId || request.ReceiverId == userId;
        }
    }
}
