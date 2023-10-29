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

        GetMessagesByLastId,
        GetMessageByOffset,
        GetMessagesById,
        NewMessage,

        GetFriendRequest,
        GetFriendRequestMany,
        ResponseFriendRequest, 
        GetAllFriendRequestForUser,
        SendFriendRequest,

        CreateChat,
        GetChat,
        GetChatMany,
        GetChatsByDialogue,

        GetAllChatInvitesForUser,
        SendChatInvite,
        GetChatInvite,
        GetChatInviteMany,
        ChatInviteRespose,
        
    }
}
