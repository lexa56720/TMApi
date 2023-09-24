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
            if (DataConstraints.IsNameLegal(name))
                return await Requester.PostAsync<Chat, ChatCreationRequest>
                                (RequestHeaders.CreateChat, new ChatCreationRequest(name, membersId));
            return null;
        }
        public async Task<Chat?> GetChat(int chatId)
        {
            return await Requester.PostAsync<Chat, IntContainer>
                (RequestHeaders.GetChat, new IntContainer(chatId));
        }
        public async Task<Chat[]> GetChat(int[] chatIds)
        {
            var chats = await Requester.PostAsync<SerializableArray<Chat>, IntArrayContainer>
                (RequestHeaders.GetChatMany, new IntArrayContainer(chatIds));
            if (chats == null)
                return Array.Empty<Chat>();
            return chats.Items;
        }


        public async Task<bool> SendChatInvite(int chatId, int toUserId)
        {
            return await Requester.GetAsync
                (RequestHeaders.SendChatInvite, new ChatInvite(chatId, toUserId, Api.Id));
        }

        public async Task<ChatInvite?> GetChatInvite(int inviteId)
        {
            return await Requester.PostAsync<ChatInvite, IntContainer>
                (RequestHeaders.GetChatInvite, new IntContainer(inviteId));
        }
        public async Task<ChatInvite[]> GetChatInvite(int[] inviteId)
        {
            var invites = await Requester
                .PostAsync<SerializableArray<ChatInvite>, IntArrayContainer>
                (RequestHeaders.GetChatInviteMany, new IntArrayContainer(inviteId));

            if (invites == null)
                return Array.Empty<ChatInvite>();
            return invites.Items;
        }
        public async Task<int[]> GetAllInvites(int userId)
        {
            var invites = await Requester.PostAsync<IntArrayContainer, IntContainer>
                (RequestHeaders.GetAllChatInvitesForUser, new IntContainer(userId));

            if (invites == null)
                return Array.Empty<int>();
            return invites.Values;
        }
        public async Task<bool> SendChatInviteResponse(ChatInvite invite, bool isAccepted)
        {
            return await SendChatInviteResponse(invite.Id, isAccepted);
        }
        public async Task<bool> SendChatInviteResponse(int inviteId, bool isAccepted)
        {
            var response = new RequestResponse(inviteId, isAccepted);

            return await Requester.GetAsync(RequestHeaders.ChatInviteRespose, response);
        }


        public async Task<Chat[]> GetAllDialogues()
        {
            return await GetByDialogue(true);
        }
        public async Task<Chat[]> GetAllNonDialogues()
        {
            return await GetByDialogue(false);
        }

        private async Task<Chat[]> GetByDialogue(bool isDialogue)
        {
            var chats = await Requester.PostAsync<SerializableArray<Chat>, ChatRequest>
                             (RequestHeaders.GetChatsByDialogue, new ChatRequest(isDialogue));

            if (chats == null)
                return Array.Empty<Chat>();
            return chats.Items;
        }
    }
}
