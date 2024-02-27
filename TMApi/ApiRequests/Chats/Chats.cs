using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Chats;
using ApiTypes.Shared;

namespace TMApi.ApiRequests.Chats
{
    public class Chats : BaseRequester
    {
        internal Chats(RequestSender requester, Api api) : base(requester, api)
        {
        }

        public async Task<Chat?> CreateChat(string name, int[] membersId)
        {
            if (!DataConstraints.IsNameLegal(name))
                return null;

            return await Requester.RequestAsync<Chat, ChatCreationRequest>
                         (RequestHeaders.CreateChat, new ChatCreationRequest(name, membersId));
        }
        public async Task<Chat?> GetChat(int chatId)
        {
            return await Requester.RequestAsync<Chat, IntContainer>
                (RequestHeaders.GetChat, new IntContainer(chatId));
        }
        public async Task<Chat[]> GetChat(int[] chatIds)
        {
            if (chatIds.Length == 0)
                return [];

            var chats = await Requester.RequestAsync<SerializableArray<Chat>, IntArrayContainer>
                (RequestHeaders.GetChatMany, new IntArrayContainer(chatIds));
            if (chats == null)
                return [];
            return chats.Items;
        }


        public async Task<bool> SendChatInvite(int chatId, int toUserId)
        {
            return await Requester.SendAsync
                (RequestHeaders.SendChatInvite, new ChatInvite(chatId, toUserId, Api.Id));
        }
        public async Task<ChatInvite?> GetChatInvite(int inviteId)
        {
            return await Requester.RequestAsync<ChatInvite, IntContainer>
                (RequestHeaders.GetChatInvite, new IntContainer(inviteId));
        }
        public async Task<ChatInvite[]> GetChatInvite(int[] inviteId)
        {
            if (inviteId.Length == 0)
                return [];

            var invites = await Requester.RequestAsync<SerializableArray<ChatInvite>, IntArrayContainer>
                                     (RequestHeaders.GetChatInviteMany, new IntArrayContainer(inviteId));

            if (invites == null)
                return [];
            return invites.Items;
        }
        public async Task<int[]> GetAllInvites()
        {
            var invites = await Requester.RequestAsync<IntArrayContainer, Request>
                           (RequestHeaders.GetAllChatInvitesForUser, new Request());

            if (invites == null)
                return [];
            return invites.Values;
        }
        public async Task<bool> SendChatInviteResponse(int inviteId, bool isAccepted)
        {
            var response = new RequestResponse(inviteId, isAccepted);

            return await Requester.SendAsync(RequestHeaders.ChatInviteRespose, response);
        }

        public async Task<int[]> GetAllChats()
        {
            var chats = await Requester.RequestAsync<IntArrayContainer, Request>
                                     (RequestHeaders.GetAllChats, new Request());

            if (chats == null)
                return [];
            return chats.Values;
        }
    }
}
