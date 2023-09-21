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
                return await Requester.PostRequestAsync<Chat, ChatCreationRequest>
                                (RequestHeaders.CreateChat, new ChatCreationRequest(name, membersId));
            return null;
        }

        public async Task<Chat?> GetChat(int chatId)
        {
            return await Requester.PostRequestAsync<Chat, IntContainer>
                (RequestHeaders.GetChat, new IntContainer(chatId));
        }
        public async Task<Chat[]> GetChat(int[] chatIds)
        {
            var chats = await Requester.PostRequestAsync<SerializableArray<Chat>, IntArrayContainer>
                (RequestHeaders.GetChatMany, new IntArrayContainer(chatIds));
            if (chats == null)
                return Array.Empty<Chat>();
            return chats.Items;
        }

        public async Task<bool> SendChatInvite(int chatId, int toUserId)
        {
            return await Requester.GetRequestAsync
                (RequestHeaders.SendChatInvite, new ChatInvite(chatId, toUserId, Api.Id));
        }

        public async Task<ChatInvite?> GetChatInvite(int inviteId)
        {
            return await Requester.PostRequestAsync<ChatInvite, IntContainer>
                (RequestHeaders.GetChatInvite, new IntContainer(inviteId));
        }
        public async Task<ChatInvite[]> GetChatInvite(int[] inviteId)
        {
            var invites = await Requester
                .PostRequestAsync<SerializableArray<ChatInvite>, IntArrayContainer>
                (RequestHeaders.GetChatInviteMany, new IntArrayContainer(inviteId));

            if (invites == null)
                return Array.Empty<ChatInvite>();
            return invites.Items;
        }
        public async Task<int[]> GetAllInvites(int userId)
        {
            var invites = await Requester.PostRequestAsync<IntArrayContainer, IntContainer>
                (RequestHeaders.GetAllChatInvitesForUser, new IntContainer(userId));

            if (invites == null)
                return Array.Empty<int>();
            return invites.Values;
        }
        public async Task<bool> SendChatInviteResponse(ChatInvite invite, bool isAccepted)
        {
            var response = new RequestResponse(invite.Id, isAccepted);

            return await Requester.GetRequestAsync(RequestHeaders.ChatInviteRespose, response);
        }
    }
}
