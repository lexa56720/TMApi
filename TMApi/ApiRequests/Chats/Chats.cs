using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Chats;
using ApiTypes.Shared;
using System;

namespace TMApi.ApiRequests.Chats
{
    public class Chats : BaseRequester
    {
        internal Chats(RequestSender requester, Api api) : base(requester, api)
        {
        }

        public async Task<Chat?> CreateChat(string name, int[] membersId)
        {
            var members = membersId.Distinct().ToArray();
            if (!DataConstraints.IsNameLegal(name) || members.Length < 2)
                return null;

            return await Requester.ApiRequestAsync<Chat, ChatCreationRequest>(new ChatCreationRequest(name, members));
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
            return await RequestMany(chatIds,
                (ids) => new ChatRequest(ids),
                Requester.ApiRequestAsync<SerializableArray<Chat>, ChatRequest>,
                (x) => x.Id);
        }

        public async Task<ChatInvite?> GetChatInvite(int inviteId)
        {
            var result = await GetChatInvite([inviteId]);
            if (result.Length == 0)
                return null;
            return result[0];
        }
        public async Task<ChatInvite[]> GetChatInvite(int[] inviteIds)
        {
            return await RequestMany(inviteIds,
                (ids) => new InviteRequest(ids),
                Requester.ApiRequestAsync<SerializableArray<ChatInvite>, InviteRequest>,
                (x) => x.Id);
        }
        public async Task<bool> SendChatInvite(int chatId, params int[] toUserIds)
        {
            if(toUserIds.Length==0)
                return false;
            return await Requester.ApiSendAsync(new InviteToChatRequest(chatId, toUserIds.Distinct().ToArray()));
        }
        public async Task<bool> SendChatInviteResponse(int inviteId, bool isAccepted)
        {
            return await Requester.ApiSendAsync(new ResponseToInvite(inviteId, isAccepted));
        }
        public async Task<int[]> GetAllInvites()
        {
            var invites = await Requester.ApiRequestAsync<IntArrayContainer, InviteRequestAll>(new InviteRequestAll());
            if (invites == null)
                return [];
            return invites.Values;
        }
        public async Task<int[]> GetAllChats()
        {
            var chats = await Requester.ApiRequestAsync<IntArrayContainer, ChatRequestAll>(new ChatRequestAll());

            if (chats == null)
                return [];
            return chats.Values;
        }
    }
}
