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
            var result = await GetChat([chatId]);
            if (result.Length == 0)
                return null;
            return result[0];
        }
        public async Task<Chat[]> GetChat(int[] chatIds)
        {
            var chats = await Requester.RequestAsync<SerializableArray<Chat>, IntArrayContainer>
                (RequestHeaders.GetChat, new IntArrayContainer(chatIds));
            if (chats == null)
                return [];
            return chats.Items;
        }

        public async Task<ChatInvite?> GetChatInvite(int inviteId)
        {
            var result = await GetChatInvite([inviteId]);
            if (result.Length == 0)
                return null;
            return result[0];
        }
        public async Task<ChatInvite[]> GetChatInvite(int[] inviteId)
        {
            var invites = await Requester.RequestAsync<SerializableArray<ChatInvite>, IntArrayContainer>
                                     (RequestHeaders.GetChatInvite, new IntArrayContainer(inviteId));

            if (invites == null)
                return [];
            return invites.Items;
        }

        public async Task<bool> SendChatInvite(int chatId, int toUserId)
        {
            return await Requester.SendAsync
                (RequestHeaders.SendChatInvite, new ChatInvite(chatId, toUserId, Api.Id));
        }
        public async Task<bool> SendChatInviteResponse(int inviteId, bool isAccepted)
        {
            var response = new RequestResponse(inviteId, isAccepted);

            return await Requester.SendAsync(RequestHeaders.ChatInviteRespose, response);
        }
        public async Task<int[]> GetAllInvites()
        {
            var invites = await Requester.RequestAsync<IntArrayContainer, Request>
                           (RequestHeaders.GetAllChatInvites, new Request());

            if (invites == null)
                return [];
            return invites.Values;
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
