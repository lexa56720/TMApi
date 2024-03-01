namespace ApiTypes
{
    public enum RequestHeaders
    {
        None,

        LongPoll,

        UpdateAuth,

        GetUserInfo,
        GetUser,
        ChangeName,
        SearchByName,

        GetMessagesByLastId,
        GetMessageByOffset,
        GetMessagesById,
        NewMessage,

        GetFriendRequest,
        ResponseFriendRequest, 
        GetAllFriendRequests,
        SendFriendRequest,

        CreateChat,
        GetChat,
        GetAllChats,

        GetAllChatInvites,
        SendChatInvite,
        GetChatInvite,
        ChatInviteRespose,
        
    }
}
