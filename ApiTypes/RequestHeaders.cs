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

        GetLastMessages,
        SendMessage,

        GetFriendRequest,
        GetFriendRequestMany,
        ResponseFriendRequest,
        SendFriendRequest,

        CreateChat,
        GetChat,
        GetChatMany,
        SendChatInvite,
        GetChatInvite,
        GetChatInviteMany,
    }
}
