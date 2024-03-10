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
        public static SerializableArray<FriendRequest>? GetFriendRequests(ApiData<GetFriendRequests> request)
        {
            if (!Security.IsHaveAccessToRequest(request.Data.Ids,request.UserId))
                return null;
            var requests = Friends.GetFriendRequest(request.Data.Ids);

            var filteredRequests = requests.Select(Convert).ToArray();

            if (filteredRequests.Length == 0)
                return new SerializableArray<FriendRequest>([]);

            return new SerializableArray<FriendRequest>(filteredRequests);
        }

        public static void AddFriendRequest(ApiData<FriendRequest> data)
        {
            Friends.RegisterFriendRequest(data.UserId, data.Data.ToId);
        }
        public static void FriendRequestResponse(ApiData<RequestResponse> data)
        {
            Friends.FriendRequestResponse(data.Data.RequestId, data.Data.IsAccepted);
        }

        public static IntArrayContainer? GetAllFriendRequests(ApiData<GetAllFriendRequests> userId)
        {
            return new IntArrayContainer(Friends.GetAllForUser(userId.UserId));
        }

        private static FriendRequest Convert(DBFriendRequest request)
        {
           return new FriendRequest(request.SenderId, request.ReceiverId, request.Id);
        }
    }
}
