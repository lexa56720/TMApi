﻿using ApiTypes.Communication.BaseTypes;
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
            if (!Security.IsHaveAccessToRequest(request.UserId, request.Data.Ids))
                return null;

            var converted = DbConverter.Convert(Friends.GetFriendRequest(request.Data.Ids).ToArray());
            if (converted.Length == 0)
                return new SerializableArray<FriendRequest>([]);

            return new SerializableArray<FriendRequest>(converted);
        }

        public static void AddFriendRequest(ApiData<FriendRequest> request)
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
        public static void FriendRequestResponse(ApiData<RequestResponse> request)
        {
            if (!Security.IsExistFriendRequest(request.Data.RequestId, request.UserId))
                return;

            var dbRequest = Friends.RemoveFriendRequest(request.Data.RequestId);
            if (request.Data.IsAccepted)
                Friends.RegisterFriends(dbRequest.SenderId, dbRequest.ReceiverId);
        }

        public static IntArrayContainer? GetAllFriendRequests(ApiData<GetAllFriendRequests> request)
        {
            return new IntArrayContainer(Friends.GetAllForUser(request.UserId));
        }

        internal static void RemoveFriend(ApiData<FriendRemoveRequest> request)
        {
            if (!Security.IsFriends(request.UserId, request.Data.FriendId))
                return;
            Friends.RemoveFriend(request.UserId, request.Data.FriendId);
        }
    }
}
