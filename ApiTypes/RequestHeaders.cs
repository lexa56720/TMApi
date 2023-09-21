namespace ApiTypes
{
    public enum RequestHeaders
    {
        None,

        LongPoll,

        UpdateAuth,

        GetUserInfo,
        GetUser,
        GetUserMany,
        ChangeName,
        SearchByName,

        GetLastMessages,
        SendMessage,

        GetFriendRequest,
        GetFriendRequestMany,
        ResponseFriendRequest, 
        GetAllFriendRequestForUser,
        SendFriendRequest,

        CreateChat,
        GetChat,
        GetChatMany,
        GetAllChatInvitesForUser,
        SendChatInvite,
        GetChatInvite,
        GetChatInviteMany,
        ChatInviteRespose
    }
}
